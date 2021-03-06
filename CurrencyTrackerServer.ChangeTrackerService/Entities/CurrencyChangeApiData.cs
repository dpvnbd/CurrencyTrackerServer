﻿using System;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.ChangeTrackerService.Entities
{
    public class CurrencyChangeApiData
    {
        public UpdateSource UpdateSource { get; set; }
        private double _percentChanged = Double.NaN;
        public string Currency { get; set; }
        public double CurrentBid { get; set; }
        public double PreviousDayBid { get; set; }

        public double PercentChanged
        {
            get
            {
                if (double.IsNaN(_percentChanged))
                {
                    _percentChanged = CalculateChange();
                }
                return _percentChanged;
            }
            set => _percentChanged = value;
        }

        private double CalculateChange()
        {
            return (CurrentBid - PreviousDayBid) / PreviousDayBid * 100;
        }
        
    }
}
