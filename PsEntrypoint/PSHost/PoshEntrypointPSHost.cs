using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation.Host;
using System.Text;
using System.Threading.Tasks;

namespace PsEntrypoint
{
    internal class PoshEntrypointPSHost : PSHost
    {
        private string m_Name;
        private Guid m_InstanceId;
        private CultureInfo m_CurrentCulture;
        private CultureInfo m_CurrentUICulture;
        private PSHostUserInterface m_UI;

        public bool ShouldExit { get; private set; }
        public int ExitCode { get; private set; }

        public PoshEntrypointPSHost()
        {
            m_InstanceId = Guid.NewGuid();
            m_Name = "PSDockerHost";
            m_CurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            m_CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            m_UI = new HostUI();
        }

        public override CultureInfo CurrentCulture => m_CurrentCulture;

        public override CultureInfo CurrentUICulture => m_CurrentUICulture;

        public override Guid InstanceId => m_InstanceId;

        public override string Name => m_Name;

        public override PSHostUserInterface UI => m_UI;

        public override Version Version => new Version(1, 0, 0, 0);

        public override void EnterNestedPrompt()
        {
        }

        public override void ExitNestedPrompt()
        {
        }

        public override void NotifyBeginApplication()
        {
        }

        public override void NotifyEndApplication()
        {
        }

        public override void SetShouldExit(int exitCode)
        {
            Console.WriteLine("SetShouldExit({0})", exitCode);
            ShouldExit = true;
            ExitCode = 0;
        }
    }

}
