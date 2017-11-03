using System;
using System.ComponentModel.DataAnnotations;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.ChangeTrackerService.Entities
{
    public class CurrencyStateEntity
    {
        public string Currency { get; set; }

        public ChangeSource ChangeSource { get; set; }

        public DateTime LastChangeTime { get; set; }
        public DateTime Created { get; set; }
        public double Threshold { get; set; }



        public static double CalculateThreshold(double incrementPercentage, double percentChange)
        {
            return ((int) (percentChange / incrementPercentage)) * incrementPercentage;
        }
    }
}