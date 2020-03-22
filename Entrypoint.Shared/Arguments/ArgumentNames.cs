using System;
using System.Collections.Generic;
using System.Text;

namespace Entrypoint.Shared
{
    internal class ArgumentNames
    {
        public const string EntrypointCommand = "--entrypoint";
        public const string EntrypointCommandShort = "-e";
        public const string EntrypointScript = "--entrypointScript";
        public const string EntrypointScriptShort = "-E";
        public const string EntrypointTimeout = "--entrypointTimeout";
        public const string EntrypointTimeoutShort = "-te";

        public const string ShutdownCommand = "--shutdown";
        public const string ShutdownCommandShort = "-s";
        public const string ShutdownScript = "--shutdownScript";
        public const string ShutdownScriptShort = "-S";
        public const string ShutdownTimeout = "--shutdownTimeout";
        public const string ShutdownTimeoutShort = "-ts";

        public const string IgnoreErrors = "--ignoreErrors";
        public const string IgnoreErrorsShort = "-ie";
    }
}
