using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

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

        Thread _loggingThread;
        ConcurrentQueue<string> _loggingQueue = new ConcurrentQueue<string>();

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

            _loggingThread = new Thread(LoggingThread);

            _loggingThread.IsBackground = true;

            _loggingThread.Start();
        }

        public void LoggingThread()
        {
            while (true)
            {
                List<string> entries = new List<string>();

                var count = _loggingQueue.Count;

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        _loggingQueue.TryDequeue(out var e);
                        entries.Add(e);
                    }

                    File.AppendAllLines(_logPath, entries);
                }

                Thread.Sleep(250);
            }
        }

        public void Log(string s)
        {
            _loggingQueue.Enqueue(s);
        }
    }
}
