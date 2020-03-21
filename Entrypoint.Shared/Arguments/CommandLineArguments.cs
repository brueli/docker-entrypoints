using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entrypoint.Shared
{
    public class CommandLineArguments
    {
        protected List<string> Args;

        protected int ArgIndex { get; private set; }

        public CommandLineArguments (string[] args)
        {
            Args = new List<string>(args);
            ArgIndex = 0;
        }

        public void Shift(int number = 1)
        {
            var numShift = Math.Min(number, Args.Count);
            while (numShift > 0)
            {
                if (Args.Count > 0)
                {
                    Args.RemoveAt(0);
                    ArgIndex++;
                }
                numShift--;
            }
        }

        public int ArgC => Args.Count;

        public T Shift<T>()
        {
            Shift();

            if (Args.Count == 0)
                return default(T);

            if (typeof(T) == typeof(int))
                return (T)Convert.ChangeType(int.Parse(Args[0]), typeof(int));

            if (typeof(T) == typeof(float))
                return (T)Convert.ChangeType(float.Parse(Args[0]), typeof(float));

            if (typeof(T) == typeof(double))
                return (T)Convert.ChangeType(double.Parse(Args[0]), typeof(double));

            if (typeof(T) == typeof(bool))
                return (T)Convert.ChangeType(bool.Parse(Args[0]), typeof(bool));

            if (typeof(T) == typeof(string))
                return (T)Convert.ChangeType(Args[0], typeof(string));

            throw new InvalidCastException($"Cannot shift parameters of type {typeof(T).Name}");
        }
    }

}
