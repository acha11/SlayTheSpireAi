using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.Commands
{
    public class WaitCommand : ICommand
    {
        public WaitCommand(int timeout)
        {
            Timeout = timeout;
        }

        public int Timeout { get; }

        public string GetString()
        {
            return CommandHelpers.FormatCommandWithOptionals("WAIT", Timeout);
        }
    }
}
