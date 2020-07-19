using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi
{
    public class CommandHelpers
    {
        public static string FormatCommandWithOptionals(string commandName, params object[] parameters)
        {
            StringBuilder sb = new StringBuilder(commandName);

            foreach (var p in parameters)
            {
                if (p != null)
                {
                    sb.Append(" " + p);
                }
            }

            return sb.ToString();
        }
    }
}
