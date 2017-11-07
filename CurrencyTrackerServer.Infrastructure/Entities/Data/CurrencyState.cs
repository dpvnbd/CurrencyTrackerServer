using System;

namespace CurrencyTrackerServer.Infrastructure.Entities.Data
{
    public class CurrencyState
    {
        public string Currency { get; set; }

        public UpdateSource UpdateSource { get; set; }

        public DateTime LastChangeTime { get; set; }
        public DateTime Created { get; set; }
        public double Threshold { get; set; }
        
        public static double CalculateThreshold(double incrementPercentage, double percentChange)
        {
            return ((int) (percentChange / incrementPercentage)) * incrementPercentage;
        }
    }
}