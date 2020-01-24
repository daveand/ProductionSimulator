using System;
using System.Collections.Generic;
using System.Text;

namespace DataGenerator
{
    public class ProductionDataModel
    {
        public string TimeStamp { get; set; }
        public int ProductionMinute { get; set; }
        public bool PlannedStop { get; set; }
        public bool Downtime { get; set; }
        public double TotalCount { get; set; }
        public double RejectCount { get; set; }
        public double CycleTime { get; set; }
        public double AckTotalCount { get; set; }
        public double AckTotalReject { get; set; }

        // OEE
        public double Availability { get; set; }
        public double Performance { get; set; }
        public double Quality { get; set; }
        public double OEE { get; set; }

    }
}
