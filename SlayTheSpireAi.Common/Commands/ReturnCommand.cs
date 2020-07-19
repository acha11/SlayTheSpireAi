using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.Commands
{
    public class ReturnCommand : ICommand
    {
        public ReturnCommand()
        {
        }

        public string GetString()
        {
            return CommandHelpers.FormatCommandWithOptionals("RETURN");
        }
    }
}
