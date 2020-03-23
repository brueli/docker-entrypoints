using System;
using System.Collections.Generic;
using System.Text;

namespace Entrypoint.Shared
{
    internal class HelpWriter
    {
        public HelpWriter()
        {
        }

        public string Indent(string value, int indent)
        {
            var indentStr = new string(' ', indent * 4);
            var result = new StringBuilder(value);
            result = result.Replace("\n", $"{indentStr}\n");
            result = result.Insert(0, indentStr);
            return result.ToString();
        }

        public void Section(string title, string content)
        {
            Console.WriteLine(".{0}", title.ToUpper());
            Console.WriteLine(Indent(content, 1));
            Console.WriteLine();
        }

        public void Argument(string[] argumentNames, string description, string value = null)
        {
            Console.WriteLine(string.Join(" | ", argumentNames) + (string.IsNullOrEmpty(value) ? string.Empty : " " + value));
            Console.WriteLine(Indent(description, 1));
            Console.WriteLine();
        }
    }
}
