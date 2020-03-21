using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsEntrypoint
{
    public interface IEntrypointState
    {
        bool Shutdown { get; }

        void RequestShutdown();
    }
}
