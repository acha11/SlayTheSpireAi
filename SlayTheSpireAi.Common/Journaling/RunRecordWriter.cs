using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SlayTheSpireAi.Common.Journaling
{
    public static class RunRecordWriter
    {
        static string _exeDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public static void Write(RunRecord record)
        {
            const string RunHistory = "RunHistory";

            Directory.CreateDirectory(Path.Combine(_exeDirectory, RunHistory));

            File.WriteAllText(
                Path.Combine(
                    _exeDirectory,
                    RunHistory,
                    "run-" + record.StartTime.ToString("yyyy-MM-dd-hh-mm-ss") + ".json"),
                JsonConvert.SerializeObject(record, Formatting.Indented)
            );
        }
    }
}
