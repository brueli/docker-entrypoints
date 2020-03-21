using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entrypoint
{
    public class EntrypointArgs : Entrypoint.Shared.CommandLineArguments
    {
        public string EntrypointCommand;
        public string EntrypointArguments;
        public string StopCommand;
        public string StopArguments;

        public int StopTimeout = 2000;
        public int ShutdownTimeout = 8000;
        
        public EntrypointArgs(string[] args)
            : base(args)
        {
            var argPos = 0; // none
            var entrypointArgs = new StringBuilder();
            var stopArgs = new StringBuilder();

            while (ArgC > 0)
            {
                if (Args[0].Equals("--entrypoint") || Args[0].Equals("-e"))
                {
                    EntrypointCommand = Shift<string>();
                    argPos = 0;
                }
                else if (Args[0].Equals("--stop") || Args[0].Equals("-s"))
                {
                    StopCommand = Shift<string>();
                    argPos = 1;
                }
                else if (Args[0].Equals("--entrypoint-timeout") || Args[0].Equals("-te"))
                {
                    StopTimeout = Shift<int>();
                    argPos = -1;
                }
                else if (Args[0].Equals("--stop-timeout") || Args[0].Equals("-ts"))
                {
                    StopTimeout = Shift<int>();
                    argPos = -1;
                }
                else
                {
                    switch (argPos)
                    {
                        case 0:
                            if (ArgIndex == 0)
                            {
                                EntrypointCommand = Args[0];
                            }
                            else
                            {
                                entrypointArgs.Append($"{Args[0]} ");
                            }
                            break;
                        case 1:
                            stopArgs.Append($"{Args[0]} ");
                            break;
                        default:
                            Console.WriteLine("Unknown argument at position {0}: {1}", ArgIndex, Args[0]);
                            break;
                    }
                }
                Shift();
            }

            EntrypointArguments = entrypointArgs.ToString();
            StopArguments = stopArgs.ToString();
        }


    }

}
