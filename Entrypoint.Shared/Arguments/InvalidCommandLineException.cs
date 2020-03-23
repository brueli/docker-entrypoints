using System;
using System.Collections.Generic;
using System.Text;

namespace Entrypoint.Shared
{
    public class InvalidCommandLineException : ApplicationException
    {
        public InvalidCommandLineException(string message)
            : base(message)
        {
        }
    }
}
