using Entrypoint.Shared;
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
        public string ShutdownCommand;
        public string ShutdownScript;

        public int EntrypointTimeout = 2000;
        public int ShutdownTimeout = 8000;
        public bool IgnoreErrors = false;

        public bool Help = false;

        public PsEntrypointArgs(string[] args)
            : base(args)
        {
            var hadErrors = false;
            var entrypointCmd = new StringBuilder();
            var shutdownCmd = new StringBuilder();

            var argPos = 0;
            while (ArgC > 0)
            {
                if (Args[0].Equals(ArgumentNames.EntrypointCommand) || Args[0].Equals(ArgumentNames.EntrypointCommandShort))
                {
                    entrypointCmd.Append($"{Shift<string>()} ");
                    argPos = 0;
                }
                if (Args[0].Equals(ArgumentNames.EntrypointScript) || Args[0].Equals(ArgumentNames.EntrypointScriptShort))
                {
                    EntrypointScript = Shift<string>();
                    argPos = -1;
                }
                else if (Args[0].Equals(ArgumentNames.ShutdownCommand) || Args[0].Equals(ArgumentNames.ShutdownCommandShort))
                {
                    ShutdownCommand = Shift<string>();
                    argPos = 1;
                }
                else if (Args[0].Equals(ArgumentNames.ShutdownScript) || Args[0].Equals(ArgumentNames.ShutdownScriptShort))
                {
                    ShutdownScript = Shift<string>();
                    argPos = -1;
                }
                else if (Args[0].Equals(ArgumentNames.EntrypointTimeout) || Args[0].Equals(ArgumentNames.EntrypointTimeoutShort))
                {
                    EntrypointTimeout = Shift<int>();
                    argPos = -1;
                }
                else if (Args[0].Equals(ArgumentNames.ShutdownTimeout) || Args[0].Equals(ArgumentNames.ShutdownTimeoutShort))
                {
                    ShutdownTimeout = Shift<int>();
                    argPos = -1;
                }
                else if (Args[0].Equals(ArgumentNames.IgnoreErrors) || Args[0].Equals(ArgumentNames.IgnoreErrorsShort))
                {
                    IgnoreErrors = true;
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
                            entrypointCmd.AppendFormat("{0} ", Args[0]);
                            break;
                        case 1:
                            shutdownCmd.AppendFormat("{0} ", Args[0]);
                            break;
                        default:
                            Console.WriteLine("Unknown argument at position {0}: {1}", ArgIndex, Args[0]);
                            hadErrors = true;
                            break;
                    }
                }
                Shift();
            }

            if (hadErrors)
                throw new InvalidCommandLineException(string.Join(" ", args));

            EntrypointCommand = entrypointCmd.ToString();
            ShutdownCommand = shutdownCmd.ToString();
        }
    }

}
