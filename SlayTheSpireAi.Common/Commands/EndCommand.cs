using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.Commands
{
    public class EndCommand : ICommand
    {
        public string GetString()
        {
            return "END";
        }
    }
}
