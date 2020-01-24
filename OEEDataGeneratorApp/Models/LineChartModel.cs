using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OEEDataGeneratorApp.Models
{
    public class LineChartModel
    {
        public string TimeStamp { get; set; }
        public int ProductionMinute { get; set; }
        public bool PlannedStop { get; set; }
        public bool Downtime { get; set; }
        public double TotalCount { get; set; }
        public double RejectCount { get; set; }
        public double CycleTime { get; set; }

    }
}
