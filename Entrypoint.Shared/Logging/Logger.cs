using System;
using System.Collections.Generic;
using System.Text;

namespace Entrypoint.Shared
{
    public class Logger
    {
        public Logger()
        { }

        public void WriteLog(string message)
        {
            using (new ConsoleColorScope(ConsoleColor.DarkGray, ConsoleColor.Black))
            {
                Console.WriteLine(message);
            }
        }

        public void WriteFatal(string message, Exception problem)
        {
            using (new ConsoleColorScope(ConsoleColor.Red, ConsoleColor.Black))
            {
                Console.WriteLine($"[FATAL] {message}: {problem}");
            }
        }
    }
}
