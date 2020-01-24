using System;
using System.Collections.Generic;
using System.Text;

namespace DataGenerator
{
    public class OEEDataGenerator
    {
        public double totalCount { get; set; }
        public double rejectCount { get; set; }
        public bool enableIdealRunRate { get; set; }
        public double idealRunRate { get; set; }
        public int simInterval { get; set; }
        public double rejectPercentageMin { get; set; }
        public double rejectPercentageMax { get; set; }
        public bool plannedStop { get; set; }
        public bool unplannedStop { get; set; }

        private static readonly Random random = new Random();

        public List<ProductionDataModel> GenerateOEEValues(
            double shiftLength,
            double totalBreakTime, 
            double downtime,
            double actualCycleTimeMin,
            double actualCycleTimeMax,
            double actualRunRateMin,
            double actualRunRateMax,
            bool radioRunRate,
            double rejectPercentageMin,
            double rejectPercentageMax)
        {

            List<ProductionDataModel> pData = new List<ProductionDataModel>();
            DateTime currentTime = new DateTime(2019, 5, 1, 8, 0, 0);

            for (var i = 0; i < shiftLength; i++ )
            {
                DateTime timeStamp = currentTime.AddMinutes(i);
                double cycleTimeCalculated;
                double totalCount;

                if (radioRunRate)
                {
                    cycleTimeCalculated = RandomNumberBetween(actualRunRateMin, actualRunRateMax);
                    totalCount = cycleTimeCalculated;
                } else
                {
                    cycleTimeCalculated = RandomNumberBetween(actualCycleTimeMin, actualCycleTimeMax);
                    totalCount = 60 / cycleTimeCalculated;

                }


                
                double rejectCount = totalCount * RandomNumberBetween(rejectPercentageMin / 100, rejectPercentageMax / 100);

                // Break one
                double breakTime = totalBreakTime / 2;
                if (i > (shiftLength * 0.25) && i < (shiftLength * 0.25) + breakTime)
                {
                    plannedStop = true;
                    cycleTimeCalculated = 0;
                    totalCount = 0;
                    rejectCount = 0;
                } else
                {
                    plannedStop = false;
                    // Break two
                    if (i > (shiftLength * 0.75) && i < (shiftLength * 0.75) + breakTime)
                    {
                        plannedStop = true;
                        cycleTimeCalculated = 0;
                        totalCount = 0;
                        rejectCount = 0;
                    }
                    else
                    {
                        plannedStop = false;
                    }
                }


                // Downtime
                if (i > (shiftLength * 0.5) && i < (shiftLength * 0.5) + downtime)
                {
                    unplannedStop = true;
                    cycleTimeCalculated = 0;
                    totalCount = 0;
                    rejectCount = 0;
                }
                else
                {
                    unplannedStop = false;
                }


                pData.Add(new ProductionDataModel
                {
                    TimeStamp = timeStamp.ToString().Substring(11),
                    ProductionMinute = i,
                    PlannedStop = plannedStop,
                    Downtime = unplannedStop,
                    TotalCount = totalCount,
                    RejectCount = rejectCount,
                    CycleTime = cycleTimeCalculated
                });

            }

            return pData;

        }

        private static double RandomNumberBetween(double minValue, double maxValue)
        {
            var next = random.NextDouble();

            return minValue + (next * (maxValue - minValue));
        }
    }
}
