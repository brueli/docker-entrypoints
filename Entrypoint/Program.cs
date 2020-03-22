using Entrypoint.Shared;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Entrypoint
{
    class Program
    {
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
        ///  Event set when the process is terminated.
        /// </summary>
        private static readonly ManualResetEvent shutdownRequested;

        /// <summary>
        /// Event set when the process terminates.
        /// </summary>
        private static readonly ManualResetEvent entrypointTerminated;

        /// <summary>
        /// Event set when main terminates.
        /// </summary>
        private static readonly ManualResetEvent mainTerminated;

        /// <summary>
        /// Close handler delegate. Stored on a static variable to prevent early garbage collection.
        /// </summary>
        private static readonly ConsoleCloseHandler consoleCloseHandler;

        /// <summary>
        /// Command line arguments
        /// </summary>
        private static EntrypointArgs cliArgs;

        /// <summary>
        /// Logger.
        /// </summary>
        private static Logger logger;

        /// <summary>
        /// Static constructor
        /// </summary>
        static Program()
        {
            // Do this initialization here to avoid polluting Main() with it
            // also this is a great place to initialize multiple static
            // variables.
            shutdownRequested = new ManualResetEvent(false);
            entrypointTerminated = new ManualResetEvent(false);
            mainTerminated = new ManualResetEvent(false);
            consoleCloseHandler = new ConsoleCloseHandler(OnConsoleCloseEvent);
            logger = new Logger();
        }

        /// <summary>
        /// The main console entry point.
        /// </summary>
        /// <param name="args">The commandline arguments.</param>
        private static void Main(string[] args)
        {
            SetConsoleCtrlHandler(consoleCloseHandler, true);

            cliArgs = new EntrypointArgs(args);

            // write banner
            logger.WriteBanner();

            // print entrypoint information
            logger.WriteLog($"starting entrypoint: {cliArgs.EntrypointCommand} {cliArgs.EntrypointArguments}");

            // run entrypoint binary
            var entrypoint = Process.Start(cliArgs.EntrypointCommand, cliArgs.EntrypointArguments);

            // wait for the kill switch
            while (!shutdownRequested.WaitOne(0))
            {
                // while waiting for the kill switch, wait for the entrypoint process to exit
                if (entrypoint.WaitForExit(100))
                {
                    // if entrypoint terminates, activate the kill switch
                    logger.WriteLog("entrypoint terminated");
                    shutdownRequested.Set();
                }
            }

            // wait for the kill switch
            shutdownRequested.WaitOne();

            // test if the entrypoint is still running
            if (!entrypoint.HasExited)
            {
                // stop the entrypoint
                logger.WriteLog("stopping entrypoint (graceful)");
                entrypoint.CloseMainWindow();

                // if the entrypoint does not respond after 10 seconds, kill the entrypoint process.
                if (!entrypoint.WaitForExit(cliArgs.StopTimeout))
                {
                    entrypoint.Kill();
                    logger.WriteLog("entrypoint killed");
                }
                // otherwise the container has stopped gracefully
                else
                {
                    logger.WriteLog("entrypoint stopped gracefully");
                }
            }
            
            // send "entrypoint terminated" signal
            entrypointTerminated.Set();

            // invoke shutdown command, if any.
            if (!string.IsNullOrWhiteSpace(cliArgs.StopCommand))
            { 
                var shutdown = Process.Start(cliArgs.StopCommand, cliArgs.StopArguments);
                if (!shutdown.WaitForExit(cliArgs.ShutdownTimeout))
                {
                    shutdown.Kill();
                    logger.WriteLog("Shutdown aborted");
                }
                else
                {
                    logger.WriteLog("Shutdown completed");
                }
            }
            
#if DEBUG
            // wait for console readline before exit
            Console.WriteLine("<Press return to exit>");
            Console.ReadLine();
#endif

            // send "main terminated" signal
            mainTerminated.Set();
        }

        /// <summary>
        /// Method called when the user presses Ctrl-C
        /// </summary>
        /// <param name="reason">The close reason</param>
        private static bool OnConsoleCloseEvent(int reason)
        {
            // Signal termination
            shutdownRequested.Set();

            // Wait for cleanup
            mainTerminated.WaitOne();

            // Don't run other handlers, just exit.
            return true;
        }

    }

}
