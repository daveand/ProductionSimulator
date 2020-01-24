using System;

namespace DataGenerator
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("OEE Data Generator\n");

            OEEDataGenerator g = new OEEDataGenerator();
            //g.GenerateOEEValues();

            OEECalculator oee = new OEECalculator
            {
                ShiftLength = 480,
                TotalBreakTime = 60,
                Downtime = 47, // Contains Unplanned Stops (e.g., Breakdowns) or Planned Stops (e.g., Changeovers)
                TotalCount = 19271,
                RejectCount = 423,
                IdealCycleTime = 1.0, // Defined in seconds
                EnableIdealRunRate = false, // Enable to calculate performance according to run rate instead of cycle time
                IdealRunRate = 70 // Parts per minute

            };


            var availability = oee.CalculateAvailability();
            var performance = oee.CalculatePerformance();
            var quality = oee.CalculateQuality();

            double oeeCalculated = availability * performance * quality;

            Console.WriteLine();
            Console.WriteLine($"Availability: {(availability * 100).ToString("0.00")}%");
            Console.WriteLine($"Performance: {(performance * 100).ToString("0.00")}%");
            Console.WriteLine($"Quality: {(quality * 100).ToString("0.00")}%");
            Console.WriteLine("---");
            Console.WriteLine($"OEE: {(oeeCalculated * 100).ToString("0.00")}%");


            Console.ReadLine();
        }
    }
}
