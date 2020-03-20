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
        private HostRawUI m_RawUI;

        public HostUI()
        {
            //m_RawUI = new HostRawUI();
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
            Console.WriteLine(value);
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.Write(value);
        }

        public override void WriteDebugLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine($"[DEBUG] {message}");
        }

        public override void WriteErrorLine(string value)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine($"[ERROR] {value}");
        }

        public override void WriteLine(string value)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(value);
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {

        }

        public override void WriteVerboseLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine($"[VERBOSE] {message}");
        }

        public override void WriteWarningLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine($"[WARNING] {message}");

        }
    }

}
