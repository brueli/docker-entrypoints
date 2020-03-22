using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsEntrypoint
{
    public class PsEntrypointArgs : Entrypoint.Shared.CommandLineArguments
    {
        public string EntrypointCommand;
        public string EntrypointScript;
        public string StopCommand;
        public string StopScript;

        public int StopTimeout = 2000;
        public bool IgnoreFatalErrors = false;

        public PsEntrypointArgs(string[] args)
            : base(args)
        {
            var entrypointCmd = new StringBuilder();
            var stopCmd = new StringBuilder();

            var argPos = 0;
            while (ArgC > 0)
            {
                if (Args[0].Equals("--entrypoint") || Args[0].Equals("-e"))
                {
                    entrypointCmd.Append($"{Shift<string>()} ");
                    argPos = 0;
                }
                if (Args[0].Equals("--entrypointScript") || Args[0].Equals("-E"))
                {
                    EntrypointScript = Shift<string>();
                    argPos = -1;
                }
                else if (Args[0].Equals("--stop") || Args[0].Equals("-s"))
                {
                    StopCommand = Shift<string>();
                    argPos = 1;
                }
                else if (Args[0].Equals("--stopScript") || Args[0].Equals("-S"))
                {
                    StopScript = Shift<string>();
                    argPos = -1;
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
                else if (Args[0].Equals("--ignore-fatal-errors") || Args[0].Equals("-ife"))
                {
                    IgnoreFatalErrors = true;
                    argPos = -1;
                }
                else
                {
                    switch (argPos)
                    {
                        case 0:
                            entrypointCmd.AppendFormat("{0} ", Args[0]);
                            break;
                        case 1:
                            stopCmd.AppendFormat("{0} ", Args[0]);
                            break;
                        default:
                            Console.WriteLine("Unknown argument at position {0}: {1}", ArgIndex, Args[0]);
                            break;
                    }
                }
                Shift();
            }

            EntrypointCommand = entrypointCmd.ToString();
            StopCommand = entrypointCmd.ToString();
        }
    }

}
