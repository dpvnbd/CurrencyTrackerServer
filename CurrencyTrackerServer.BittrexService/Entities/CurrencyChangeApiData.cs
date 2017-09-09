using System;

namespace CurrencyTrackerServer.ChangeTrackerService.Entities
{
    public class CurrencyChangeApiData
    {
        public ChangeSource ChangeSource { get; set; }
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
