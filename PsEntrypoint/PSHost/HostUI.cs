using Entrypoint.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PsEntrypoint
{

    internal class HostUI : PSHostUserInterface
    {
        private readonly PSHostRawUserInterface m_RawUI = null;
        private Dictionary<int, ConsoleColorSet> m_ConsoleColors;

        private const int DefaultColor = 0;
        private const int ErrorColor = 1;
        private const int WarningColor = 2;
        private const int VerboseColor = 3;
        private const int InfoColor = 4;
        private const int DebugColor = 5;

        public HostUI()
        {
            m_ConsoleColors = new Dictionary<int, ConsoleColorSet>();
            m_ConsoleColors[DefaultColor] = new ConsoleColorSet { ForegroundColor = ConsoleColor.Gray, BackgroundColor = ConsoleColor.Black };
            m_ConsoleColors[ErrorColor] = new ConsoleColorSet { ForegroundColor = ConsoleColor.Red, BackgroundColor = ConsoleColor.Black };
            m_ConsoleColors[WarningColor] = new ConsoleColorSet { ForegroundColor = ConsoleColor.Yellow, BackgroundColor = ConsoleColor.Black };
            m_ConsoleColors[VerboseColor] = new ConsoleColorSet { ForegroundColor = ConsoleColor.White, BackgroundColor = ConsoleColor.Black };
            m_ConsoleColors[InfoColor] = new ConsoleColorSet { BackgroundColor = ConsoleColor.Gray, ForegroundColor = ConsoleColor.Black };
            m_ConsoleColors[DebugColor] = new ConsoleColorSet { BackgroundColor = ConsoleColor.DarkGray, ForegroundColor = ConsoleColor.Black };
        }

        public override PSHostRawUserInterface RawUI => m_RawUI;

        public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
        {
            return new Dictionary<string, PSObject>();
        }

        public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
        {
            return defaultChoice;
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            return null;
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            return null;
        }

        public override string ReadLine()
        {
            return Console.ReadLine();
        }

        public override SecureString ReadLineAsSecureString()
        {
            return null;
        }

        public override void Write(string value)
        {
            using (ConsoleColorScope.CreateFrom(m_ConsoleColors[DefaultColor]))
            {
                Console.WriteLine(value);
            }
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            using (new ConsoleColorScope(foregroundColor, backgroundColor))
            {
                Console.Write(value);
            }
        }

        public override void WriteDebugLine(string message)
        {
            using (ConsoleColorScope.CreateFrom(m_ConsoleColors[DebugColor]))
            {
                Console.WriteLine($"[DEBUG] {message}");
            }
        }

        public override void WriteErrorLine(string value)
        {
            using (ConsoleColorScope.CreateFrom(m_ConsoleColors[ErrorColor]))
            {
                Console.WriteLine($"[ERROR] {value}");
            }
        }

        public override void WriteLine(string value)
        {
            using (ConsoleColorScope.CreateFrom(m_ConsoleColors[DefaultColor]))
            {
                Console.WriteLine(value);
            }
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {

        }

        public override void WriteVerboseLine(string message)
        {
            using (ConsoleColorScope.CreateFrom(m_ConsoleColors[VerboseColor]))
            {
                Console.WriteLine($"[VERBOSE] {message}");
            }
        }

        public override void WriteWarningLine(string message)
        {
            using (ConsoleColorScope.CreateFrom(m_ConsoleColors[WarningColor]))
            {
                Console.WriteLine($"[WARNING] {message}");
            }
        }
    }

}
