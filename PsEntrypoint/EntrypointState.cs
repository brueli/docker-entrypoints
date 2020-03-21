using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsEntrypoint
{
    public class EntrypointState : IEntrypointState
    {
        public EntrypointState() { }

        public Action<Exception> ReportFatalErrorCallback;

        public bool Shutdown { get; set; }

        public void RequestShutdown()
        {
            Shutdown = true;
        }

        public void ReportFatalError(Exception problem)
        {
            ReportFatalErrorCallback?.Invoke(problem);
        }
    }
}
