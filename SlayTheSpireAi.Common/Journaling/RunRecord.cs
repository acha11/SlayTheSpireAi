using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.Journaling
{
    public class RunRecord
    {
        public long Seed { get; set; }
        public string Character { get; set; }
        public int AscensionLevel { get; set; }

        public List<RunEvent> Events { get; set; }
        public DateTime StartTime { get; set; }

        public RunRecord()
        {
            Events = new List<RunEvent>();
        }

        public void AddEvent(RunEvent evt)
        {
            Events.Add(evt);
        }
    }
}
