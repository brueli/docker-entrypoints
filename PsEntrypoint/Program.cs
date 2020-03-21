using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PsEntrypoint
{
    class Program
    {
        /// <summary>
        /// Shared container state object
        /// </summary>
        private static EntrypointState EntrypointState;

        /// <summary>
        /// Shutdown request event.
        /// Set by Ctrl+C handler or when using "$container.RequestShutdown()" in the entrypoint script.
        /// Initiates entrypoint shutdown.
        /// </summary>
        private static ManualResetEvent shutdownRequested;

        /// <summary>
        /// Entrypoint terminated event.
        /// Is set by the powershell thread to signal that the entrypoint script has terminated.
        /// </summary>
        private static ManualResetEvent entrypointTerminated;

        /// <summary>
        /// Main thread terminated event.
        /// Is set by the main thread to signal its termination to the Ctrl+C handler.
        /// </summary>
        private static ManualResetEvent mainTerminated;

        /// <summary>
        /// Command line arguments for PSEntrypoint.exe
        /// </summary>
        private static PsEntrypointArgs cliArgs;

        /// <summary>
        /// Adds or removes an application-defined HandlerRoutine function from the list of handler functions for the calling process
        /// </summary>
        /// <param name="handler">A pointer to the application-defined HandlerRoutine function to be added or removed. This parameter can be NULL.</param>
        /// <param name="add">If this parameter is TRUE, the handler is added; if it is FALSE, the handler is removed.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(ConsoleCloseHandler handler, bool add);

        /// <summary>
        /// The console close handler delegate.
        /// </summary>
        /// <param name="closeReason">
        /// The close reason.
        /// </param>
        /// <returns>
        /// True if cleanup is complete, false to run other registered close handlers.
        /// </returns>
        private delegate bool ConsoleCloseHandler(int closeReason);

        /// <summary>
        /// The console close handler. 
        /// Must be a static reference, otherwise it will be collected by the garbage collector too early.
        /// </summary>
        private static ConsoleCloseHandler consoleCloseHandler;

        /// <summary>
        /// The powershell thread to run the entrypoint and shutdown scripts
        /// </summary>
        private static Thread powershellThread;

        /// <summary>
        /// Logger to send messages to the console.
        /// </summary>
        private static Logger logger;

        /// <summary>
        /// Variable name for Entrypoint State object in entrypoint scripts.
        /// </summary>
        const string EntrypointVariableName = "entrypoint";

        /// <summary>
        /// Description for the Entrypoint State object variable.
        /// </summary>
        const string EntrypointVariableDescription = "Entrypoint state object to interact with the entrypoint";

        static Program()
        {
            EntrypointState = new EntrypointState();
            shutdownRequested = new ManualResetEvent(false);
            entrypointTerminated = new ManualResetEvent(false);
            mainTerminated = new ManualResetEvent(false);
            logger = new Logger();
            powershellThread = new Thread(new ParameterizedThreadStart(PowershellThread));
            consoleCloseHandler = new ConsoleCloseHandler(OnConsoleCloseEvent);
        }

        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(consoleCloseHandler, true);
            EntrypointState.ReportFatalErrorCallback = (problem) => {
                logger.WriteFatal("Fatal error in entrypoint", problem);
                if (!cliArgs.IgnoreFatalErrors)
                { 
                    EntrypointState.RequestShutdown();
                }
            };

            cliArgs = new PsEntrypointArgs(args);

            powershellThread.Start(cliArgs);

            while (!entrypointTerminated.WaitOne(0))
            {
                if (shutdownRequested.WaitOne(0) == false)
                {
                    Thread.Sleep(100);
                }
                else if (EntrypointState.Shutdown)
                {
                    shutdownRequested.Set();
                }
                else
                {
                    EntrypointState.Shutdown = true;
                    logger.WriteLog("TERM signal received. Waiting for entrypoint to stop...");
                    if (!entrypointTerminated.WaitOne(cliArgs.StopTimeout))
                    {
                        logger.WriteLog($"entrypoint did not stop after {cliArgs.StopTimeout}ms. Forcing termination...");
                        powershellThread.Interrupt();
                    }
                    break;
                }
            }

            // wait for powershell thread to terminate
            entrypointTerminated.WaitOne();

#if DEBUG
            Console.WriteLine("<Press return to exit>");
            Console.ReadLine();
#endif

            mainTerminated.Set();
        }

        static void PowershellThread(object state)
        {
            var cliArgs = (PsEntrypointArgs)state;

            if (!string.IsNullOrWhiteSpace(cliArgs.EntrypointScript))
            {
                cliArgs.EntrypointCommand = System.IO.File.ReadAllText(cliArgs.EntrypointScript);
            }

            if (!string.IsNullOrWhiteSpace(cliArgs.ShutdownScript))
            {
                cliArgs.ShutdownCommand = System.IO.File.ReadAllText(cliArgs.ShutdownScript);
            }

            var initialSessionState = InitialSessionState.CreateDefault();
            initialSessionState.Variables.Add(new SessionStateVariableEntry(EntrypointVariableName, (IEntrypointState)EntrypointState, EntrypointVariableDescription));

            var psHost = new PoshEntrypointPSHost();

            using (var runspace = RunspaceFactory.CreateRunspace(psHost, initialSessionState))
            {
                PowerShell powershell;
                runspace.Open();

                // run entrypoint command
                using (powershell = PowerShell.Create())
                {
                    powershell.Runspace = runspace;
                    powershell.AddScript(cliArgs.EntrypointCommand);
                    var entrypointResult = powershell.BeginInvoke();
                    logger.WriteLog("Entrypoint started");
                    Thread.Sleep(5);
                    try
                    {
                        foreach (var result in powershell.EndInvoke(entrypointResult))
                        {
                            Console.WriteLine(result.ToString());
                        }
                    }
                    catch (ThreadInterruptedException)
                    {
                        logger.WriteLog("Entrypoint interrupted. Terminating...");
                        powershell.Stop();
                    }
                    catch
                    {
                        logger.WriteLog("Entrypoint crashed");
                    }
                    finally
                    {
                        logger.WriteLog("Entrypoint stopped");
                    }
                }

                // run shutdown command
                if (!string.IsNullOrWhiteSpace(cliArgs.ShutdownCommand))
                {
                    using (powershell = PowerShell.Create())
                    {
                        powershell.Runspace = runspace;
                        powershell.AddScript(cliArgs.ShutdownCommand);
                        var shutdownResult = powershell.BeginInvoke();
                        Console.WriteLine("Shutdown initiated");
                        Thread.Sleep(5);
                        try
                        {
                            foreach (var result in powershell.EndInvoke(shutdownResult))
                            {
                                Console.WriteLine(result.ToString());
                            }
                            logger.WriteLog("Shutdown completed");
                        }
                        catch (Exception problem)
                        {
                            logger.WriteLog($"Shutdown crashed: {problem}");
                        }
                        finally
                        {
                            logger.WriteLog("Shutdown ended");
                        }
                    }
                }
                runspace.Close();
            }

            entrypointTerminated.Set();
        }

        private static bool OnConsoleCloseEvent(int reason)
        {
            shutdownRequested.Set();
            mainTerminated.WaitOne();
            return true;
        }

    }

    public class Logger
    {
        public Logger()
        { }

        public void WriteLog(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(message);
        }

        public void WriteFatal(string message, Exception problem)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine($"[FATAL] {message}: {problem}");
        }
    }
}
