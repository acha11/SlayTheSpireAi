using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace SlayTheSpireAi
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger l = new Logger();

            Ai ai = new Ai(l);

            try
            {
                ai.Run();
            }
            catch (Exception ex)
            {
                l.Log("Exception: " + ex.ToString());
            }

            l.Log("Shutting down");
        }
    }
}
