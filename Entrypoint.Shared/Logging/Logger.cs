using System;
using System.Collections.Generic;
using System.Text;

namespace Entrypoint.Shared
{
    public class Logger
    {
        public Logger()
        { }

        public void WriteBanner()
        {
            var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            var thisName = assemblyName.Name;
            var thisVersion = assemblyName.Version.ToString();
#if DEBUG
            Console.WriteLine("{0} v.{1}(debug) - (c)2020, Ramon Brülisauer <ramon.bruelisauer@gmail.com>, MIT license", thisName, thisVersion);
#else
            Console.WriteLine("{0} v.{1} - (c)2020, Ramon Brülisauer <ramon.bruelisauer@gmail.com>, MIT license", thisName, thisVersion);
#endif
        }

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
