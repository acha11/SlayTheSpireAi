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
        void Shutdown();
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string s)
        {
            Console.WriteLine(s);
        }

        public void Shutdown()
        {
        }
    }

    public class FileLogger : ILogger
    {
        string _exeDirectory;
        string _logPath;

        Thread _loggingThread;
        ConcurrentQueue<string> _loggingQueue = new ConcurrentQueue<string>();

        bool _shutdownCommandReceived = false;
        bool _shutdown = false;

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
            while (!_shutdownCommandReceived)
            {
                Thread.Sleep(250);

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
            }

            _shutdown = true;
        }

        public void Log(string s)
        {
            _loggingQueue.Enqueue(s);
        }

        public void Shutdown()
        {
            _shutdownCommandReceived = true;

            while (!_shutdown)
            {
                Thread.Sleep(100);
            }
        }
    }
}
