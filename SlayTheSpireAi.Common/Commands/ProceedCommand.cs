using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.Commands
{
    public class ProceedCommand : ICommand
    {
        public string GetString()
        {
            return "PROCEED";
        }
    }
}
