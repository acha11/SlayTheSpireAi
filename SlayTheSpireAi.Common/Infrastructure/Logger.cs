using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SlayTheSpireAi.Infrastructure
{
    public interface ILogger
    {
        void Log(string s);
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string s)
        {
            Console.WriteLine(s);
        }
    }

    public class FileLogger : ILogger
    {
        string _exeDirectory;
        string _logPath;

        public FileLogger()
        {
            _exeDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            _logPath = Path.Combine(_exeDirectory, "ai.log");

            if (File.Exists(_logPath))
            {
                string nameForOldFile = _logPath + ".previous.log";

                File.Delete(nameForOldFile);
                File.Move(_logPath, nameForOldFile);
            }
        }

        public void Log(string s)
        {
            File.AppendAllLines(_logPath, new string[] { s });
        }
    }
}
