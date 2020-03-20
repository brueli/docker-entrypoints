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
        private static readonly ManualResetEvent TerminationRequestedEvent;

        /// <summary>
        /// Event set when the process terminates.
        /// </summary>
        private static readonly ManualResetEvent TerminationCompletedEvent;

        /// <summary>
        /// Static constructor
        /// </summary>
        static Program()
        {
            // Do this initialization here to avoid polluting Main() with it
            // also this is a great place to initialize multiple static
            // variables.
            TerminationRequestedEvent = new ManualResetEvent(false);
            TerminationCompletedEvent = new ManualResetEvent(false);
            SetConsoleCtrlHandler(OnConsoleCloseEvent, true);
        }

        /// <summary>
        /// The main console entry point.
        /// </summary>
        /// <param name="args">The commandline arguments.</param>
        private static void Main(string[] args)
        {
            // print entrypoint information
            var entrypointStr = string.Join(" ", args);
            Console.WriteLine("starting entrypoint: {0}", entrypointStr);

            // run entrypoint binary
            var binary = args.First();
            var remainingArgs = args.Skip(1).ToArray();
            var entrypoint = Process.Start(binary, string.Join(" ", remainingArgs));

            // wait for the kill switch
            while (!TerminationRequestedEvent.WaitOne(0))
            {
                // while waiting for the kill switch, wait for the entrypoint process to exit
                if (entrypoint.WaitForExit(100))
                {
                    // if entrypoint terminates, activate the kill switch
                    TerminationRequestedEvent.Set();
                }
            }

            // wait for the kill switch
            TerminationRequestedEvent.WaitOne();

            // stop the entrypoint
            Console.WriteLine("stopping entrypoint (graceful)");
            entrypoint.CloseMainWindow();

            // if the entrypoint does not respond after 10 seconds, kill the entrypoint process.
            if (!entrypoint.WaitForExit(10000))
            {
                entrypoint.Kill();
                Console.WriteLine("entrypoint killed");
            }
            // otherwise the container has stopped gracefully
            else
            {
                Console.WriteLine("entrypoint stopped gracefully");
            }

#if DEBUG
            // wait for console readline before exit
            Console.WriteLine("<Press return to exit>");
            Console.ReadLine();
#endif

            // Set this to terminate immediately (if not set, the OS will eventually kill the process)
            TerminationCompletedEvent.Set();
        }

        /// <summary>
        /// Method called when the user presses Ctrl-C
        /// </summary>
        /// <param name="reason">The close reason</param>
        private static bool OnConsoleCloseEvent(int reason)
        {
            // Signal termination
            TerminationRequestedEvent.Set();

            // Wait for cleanup
            TerminationCompletedEvent.WaitOne();

            // Don't run other handlers, just exit.
            return true;
        }

    }

}
