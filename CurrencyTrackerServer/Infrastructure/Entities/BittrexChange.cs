using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyTrackerServer.Infrastructure.Entities
{
    public class BittrexChange
    {
        
        //Always bitcoin
        public string ReferenceCurrency { get; set; }

        [Key]
        public string Currency { get; set; }

        public double PreviousDayBid { get; set; }

        public double CurrentBid { get; set; }

        public int Threshsold { get; set; }

        public DateTime ChangeTime { get; set; }

        public DateTime CreatedTime { get; set; }

        [NotMapped]
        public double ChangePercentage
        {
            get { return CalculateChangePercantage(PreviousDayBid, CurrentBid); }
        }


        public DateTime? LastNotifiedChange { get; set; }

        private static double CalculateChangePercantage(double value1, double value2)
        {
            return ((value2 - value1) / value1) * 100;
        }


    }
}