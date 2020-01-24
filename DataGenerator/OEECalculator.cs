using System;
using System.Collections.Generic;
using System.Text;

namespace DataGenerator
{
    public class OEECalculator
    {
        public double ShiftLength { get; set; }
        public double TotalBreakTime { get; set; }
        public double Downtime { get; set; }
        public double TotalCount { get; set; }
        public double RejectCount { get; set; }
        public double IdealCycleTime { get; set; }
        public bool EnableIdealRunRate { get; set; }
        public double IdealRunRate { get; set; }


        private double runTime;


        public double CalculateAvailability()
        {
            double plannedProductionTime = ShiftLength - TotalBreakTime;
            runTime = plannedProductionTime - Downtime;
            return runTime / plannedProductionTime;

        }

        public double CalculatePerformance()
        {
            if (EnableIdealRunRate)
            {
                return (TotalCount / runTime) / IdealRunRate;
            } else
            {
                return (IdealCycleTime * TotalCount) / (runTime * 60); //Convert runtime to seconds
            }
        }

        public double CalculateQuality()
        {
            double goodCount = TotalCount - RejectCount;
            return goodCount / TotalCount;
        }

    }
}
