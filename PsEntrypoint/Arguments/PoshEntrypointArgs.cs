using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsEntrypoint
{
    public class PoshEntrypointArgs : CommandLineArguments
    {
        public string EntrypointCommand;
        public string ShutdownCommand;
        public int StopTimeout = 2000;

        public PoshEntrypointArgs(string[] args)
            : base(args)
        {
            while (ArgC > 0)
            {
                if (Args[0].Equals("--entrypoint") || Args[0].Equals("-e"))
                {
                    EntrypointCommand = Shift<string>();
                }
                else if (Args[0].Equals("--shutdown") || Args[0].Equals("-s"))
                {
                    ShutdownCommand = Shift<string>();
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
