﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyTrackerServer.Infrastructure.Entities.Data
{
    public class CurrencyState
    {
        public string Currency { get; set; }
        public UpdateSource UpdateSource { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public DateTimeOffset LastChangeTime { get; set; }
        public DateTimeOffset Created { get; set; }
        public double Threshold { get; set; }
        
        public static double CalculateThreshold(double incrementPercentage, double percentChange)
        {
            return ((int) (percentChange / incrementPercentage)) * incrementPercentage;
        }
    }
}