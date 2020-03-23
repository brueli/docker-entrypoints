using System;
using System.Collections.Generic;
using System.Text;

namespace Entrypoint.Shared
{
    internal class ConsoleColorSet
    {
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }

        public void Apply()
        {
            Console.BackgroundColor = this.BackgroundColor;
            Console.ForegroundColor = this.ForegroundColor;
        }
    }
}
