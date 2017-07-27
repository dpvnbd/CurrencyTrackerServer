using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CurrencyTrackerServer.BittrexService.Entities
{
    public class CurrencyStateEntity
    {
        [Key]
        public string Currency { get; set; }
        public DateTime LastChangeTime { get; set; }
        public DateTime Created { get; set; }
        public double Threshold { get; set; }

        public static double CalculateThreshold(double incrementPercentage, double percentChange)
        {
            return ((int) (percentChange / incrementPercentage)) * incrementPercentage;
        }
    }
}