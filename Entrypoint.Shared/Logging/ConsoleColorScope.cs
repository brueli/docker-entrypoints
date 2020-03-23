using System;
using System.Collections.Generic;
using System.Text;

namespace Entrypoint.Shared
{
    internal class ConsoleColorScope : IDisposable
    {
        private ConsoleColor oldBg;
        private ConsoleColor oldFg;

        public ConsoleColorScope(ConsoleColor fgc, ConsoleColor bgc)
        {
            oldBg = Console.BackgroundColor;
            oldFg = Console.ForegroundColor;
            Console.BackgroundColor = bgc;
            Console.ForegroundColor = fgc;
        }

        public static ConsoleColorScope CreateFrom(ConsoleColorSet s)
        {
            return new ConsoleColorScope(s.ForegroundColor, s.BackgroundColor);
        }

        public void Dispose()
        {
            Console.BackgroundColor = oldBg;
            Console.ForegroundColor = oldFg;
        }
    }
}
