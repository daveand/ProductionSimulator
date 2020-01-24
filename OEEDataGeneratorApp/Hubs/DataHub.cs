using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataGenerator;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OEEDataGeneratorApp.Models;

namespace OEEDataGeneratorApp.Hubs
{
    public class DataHub : Hub
    {
        private readonly ILogger _logger;
        private readonly DataGenerator.OEEDataGenerator _generator;
        private bool stop;

        public DataHub(ILogger<DataHub> logger, DataGenerator.OEEDataGenerator generator)
        {
            _logger = logger;
            _generator = generator;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task StartSimulation(FormDataModel formData)
        {
            _logger.LogInformation(formData.SimInterval.ToString());
            
            var generatedData = _generator.GenerateOEEValues(
                formData.ShiftLength,
                formData.TotalBreakTime,
                formData.DownTime,
                formData.ActualCycleTimeMin,
                formData.ActualCycleTimeMax,
                formData.ActualRunRateMin,
                formData.ActualRunRateMax,
                formData.RadioRunRate,
                formData.RejectCountMin,
                formData.RejectCountMax
            );

            var ackProductionMinute = 0;
            var ackTotalCount = 0.0;
            var ackTotalReject = 0.0;
            var ackPlannedStop = 0.0;
            var ackDownTime = 0.0;

            double availability = 0.0;
            double performance = 0.0;
            double quality = 0.0;
            double oeeCalculated = 0.0;


            List<ProductionDataModel> data = new List<ProductionDataModel>();
            foreach (var point in generatedData)
            {
                if (stop)
                {
                    return;
                }

                ackProductionMinute = ackProductionMinute + 1;

                if (point.PlannedStop)
                {
                    point.TimeStamp = $"Planned Stop ({point.TimeStamp})";
                    ackPlannedStop = ackPlannedStop + 1.0;
                } else if (point.Downtime)
                {
                    point.TimeStamp = $"Downtime ({point.TimeStamp})";
                    ackDownTime = ackDownTime + 1.0;
                }

                ackTotalCount = ackTotalCount + point.TotalCount;
                point.AckTotalCount = ackTotalCount;

                ackTotalReject = ackTotalReject + point.RejectCount;
                point.AckTotalReject = ackTotalReject;



                // OEE Calculation
                OEECalculator _oee = new OEECalculator
                {
                    ShiftLength = ackProductionMinute,
                    TotalBreakTime = ackPlannedStop,
                    Downtime = ackDownTime, // Contains Unplanned Stops (e.g., Breakdowns) or Planned Stops (e.g., Changeovers)
                    TotalCount = ackTotalCount, // 19271
                    RejectCount = ackTotalReject, // 423
                    IdealCycleTime = formData.IdealCycleTime, // Defined in seconds
                    EnableIdealRunRate = formData.RadioRunRate, // Enable to calculate performance according to run rate instead of cycle time
                    IdealRunRate = formData.IdealRunRate // Parts per minute

                };


                availability = _oee.CalculateAvailability();
                performance = _oee.CalculatePerformance();
                quality = _oee.CalculateQuality();

                oeeCalculated = availability * performance * quality;


                point.Availability = availability;
                point.Performance = performance;
                point.Quality = quality;
                point.OEE = oeeCalculated;

                data.Add(point);

                               
                await Clients.All.SendAsync("StartSimulation", data);
                System.Threading.Thread.Sleep(formData.SimInterval);

            }

            ackTotalCount = 0.0;
            ackTotalReject = 0.0;


        }

        public async Task StopSimulation()
        {
            stop = true;
        }

    }
}
