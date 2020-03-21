using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsEntrypoint
{
    public class PsEntrypointArgs : CommandLineArguments
    {
        public string EntrypointCommand;
        public string EntrypointScript;
        public string ShutdownCommand;
        public string ShutdownScript;
        public int StopTimeout = 2000;

        public PsEntrypointArgs(string[] args)
            : base(args)
        {
            while (ArgC > 0)
            {
                if (Args[0].Equals("--entrypoint") || Args[0].Equals("-e"))
                {
                    EntrypointCommand = Shift<string>();
                }
                if (Args[0].Equals("--entrypointScript") || Args[0].Equals("-E"))
                {
                    EntrypointScript = Shift<string>();
                }
                else if (Args[0].Equals("--shutdown") || Args[0].Equals("-s"))
                {
                    ShutdownCommand = Shift<string>();
                }
                else if (Args[0].Equals("--shutdownScript") || Args[0].Equals("-S"))
                {
                    ShutdownScript = Shift<string>();
                }
                else if (Args[0].Equals("--stop-timeout") || Args[0].Equals("-t"))
                {
                    StopTimeout = Shift<int>();
                }
                else
                {
                    Console.WriteLine("Unknown argument at position {0}: {1}", ArgIndex, Args[0]);
                }
                Shift();
            }
        }
    }

}
