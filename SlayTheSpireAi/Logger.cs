using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SlayTheSpireAi
{
    public interface ILogger
    {
        void Log(string s);
    }

    public class Logger : ILogger
    {
        string _exeDirectory;
        string _logPath;

        public Logger()
        {
            _exeDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            _logPath = Path.Combine(_exeDirectory, "ai.log");
        }

        public void Log(string s)
        {
            File.AppendAllLines(_logPath, new string[] { s });
        }
    }
}
