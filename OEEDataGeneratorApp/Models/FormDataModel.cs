using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OEEDataGeneratorApp.Models
{
    public class FormDataModel
    {
        public double ShiftLength { get; set; }
        public double TotalBreakTime { get; set; }
        public double DownTime { get; set; }
        public bool RadioCycleTime { get; set; }
        public bool RadioRunRate { get; set; }
        public double IdealCycleTime { get; set; }
        public int IdealRunRate { get; set; }
        public double ActualCycleTimeMin { get; set; }
        public double ActualCycleTimeMax { get; set; }
        public double ActualRunRateMin { get; set; }
        public double ActualRunRateMax { get; set; }
        public int RejectCountMin { get; set; }
        public int RejectCountMax { get; set; }
        public int SimInterval { get; set; }
    }
}
