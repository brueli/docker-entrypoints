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
        static ContainerState ContainerState;
        static ManualResetEvent shutdownRequested;
        static ManualResetEvent entrypointTerminated;
        static ManualResetEvent mainTerminated;
        static PoshEntrypointArgs cliArgs;

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

        static Program()
        {
            ContainerState = new ContainerState();
            shutdownRequested = new ManualResetEvent(false);
            entrypointTerminated = new ManualResetEvent(false);
            mainTerminated = new ManualResetEvent(false);
            powershellThread = new Thread(new ParameterizedThreadStart(PowershellThread));
            consoleCloseHandler = new ConsoleCloseHandler(OnConsoleCloseEvent);
        }

        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(consoleCloseHandler, true);

            cliArgs = new PoshEntrypointArgs(args);

            powershellThread.Start(cliArgs);

            while (!entrypointTerminated.WaitOne(0))
            {
                if (shutdownRequested.WaitOne(0) == false)
                {
                    Thread.Sleep(100);
                }
                else if (ContainerState.Shutdown)
                {
                    shutdownRequested.Set();
                }
                else
                {
                    ContainerState.Shutdown = true;
                    Console.WriteLine("TERM signal received. Waiting for entrypoint to stop...");
                    if (!entrypointTerminated.WaitOne(cliArgs.StopTimeout))
                    {
                        Console.WriteLine("entrypoint did not stop after {0}ms. Forcing termination...", cliArgs.StopTimeout);
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
            var cliArgs = (PoshEntrypointArgs)state;

            var initialSessionState = InitialSessionState.CreateDefault();
            initialSessionState.Variables.Add(new SessionStateVariableEntry("container", (IContainerState)ContainerState, "Container status"));

            var psHost = new PoshEntrypointPSHost();

            using (var runspace = RunspaceFactory.CreateRunspace(psHost, initialSessionState))
            {
                runspace.Open();
                using (var powershell = PowerShell.Create())
                {
                    powershell.Runspace = runspace;

                    powershell.AddScript(cliArgs.EntrypointCommand);
                    var entrypointResult = powershell.BeginInvoke();
                    Console.WriteLine("Entrypoint started");
                    try
                    {
                        foreach (var result in powershell.EndInvoke(entrypointResult))
                        {
                            Console.WriteLine(result.ToString());
                        }
                    }
                    catch (ThreadInterruptedException)
                    {
                        Console.WriteLine("entrypoint interrupted. Terminating...");
                        powershell.Stop();
                    }
                    catch
                    {
                        Console.WriteLine("Entrypoint crashed");
                    }
                    finally
                    {
                        Console.WriteLine("Entrypoint stopped");
                    }
                    
                    if (!string.IsNullOrWhiteSpace(cliArgs.ShutdownCommand))
                    {
                        Console.WriteLine("Running shutdown script...");
                        powershell.AddScript(cliArgs.ShutdownCommand);
                        var shutdownResult = powershell.BeginInvoke();
                        Thread.Sleep(10);
                        try
                        {
                            foreach (var result in powershell.EndInvoke(entrypointResult))
                            {
                                Console.WriteLine(result.ToString());
                            }
                            Console.WriteLine("Shutdown completed");
                        }
                        catch
                        {
                            Console.WriteLine("Shutdown crashed");
                        }
                        finally
                        {
                            Console.WriteLine("Shutdown ended");
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
}
