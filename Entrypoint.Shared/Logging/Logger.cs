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
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(message);
        }

        public void WriteFatal(string message, Exception problem)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine($"[FATAL] {message}: {problem}");
        }
    }
}
