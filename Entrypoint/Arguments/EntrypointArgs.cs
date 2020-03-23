using Entrypoint.Shared;
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
        public string ShutdownCommand;
        public string ShutdownArguments;

        public int EntrypointTimeout = 2000;
        public int ShutdownTimeout = 8000;

        public bool Help = false;
        
        public EntrypointArgs(string[] args)
            : base(args)
        {
            var hadErrors = false;
            var argPos = 0; // none
            var entrypointArgs = new StringBuilder();
            var shutdownArgs = new StringBuilder();

            while (ArgC > 0)
            {
                if (Args[0].Equals(ArgumentNames.EntrypointCommand) || Args[0].Equals(ArgumentNames.EntrypointCommandShort))
                {
                    EntrypointCommand = Shift<string>();
                    argPos = 0;
                }
                else if (Args[0].Equals(ArgumentNames.ShutdownCommand) || Args[0].Equals(ArgumentNames.ShutdownCommandShort))
                {
                    ShutdownCommand = Shift<string>();
                    argPos = 1;
                }
                else if (Args[0].Equals(ArgumentNames.EntrypointTimeout) || Args[0].Equals(ArgumentNames.EntrypointTimeoutShort))
                {
                    EntrypointTimeout = Shift<int>();
                    argPos = -1;
                }
                else if (Args[0].Equals(ArgumentNames.ShutdownTimeout) || Args[0].Equals(ArgumentNames.ShutdownTimeoutShort))
                {
                    EntrypointTimeout = Shift<int>();
                    argPos = -1;
                }
                else if (Args[0].Equals(ArgumentNames.Help) || Args[0].Equals(ArgumentNames.HelpShort))
                {
                    Help = true;
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
                            shutdownArgs.Append($"{Args[0]} ");
                            break;
                        default:
                            Console.WriteLine("Invalid argument at position {0}: {1}", ArgIndex, Args[0]);
                            hadErrors = true;
                            break;
                    }
                }
                Shift();
            }

            if (hadErrors)
                throw new InvalidCommandLineException(string.Join(" ", args));

            EntrypointArguments = entrypointArgs.ToString();
            ShutdownArguments = shutdownArgs.ToString();
        }


    }

}
