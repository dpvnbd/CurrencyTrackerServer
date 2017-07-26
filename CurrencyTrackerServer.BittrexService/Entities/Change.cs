using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyTrackerServer.BittrexService.Entities
{
    public class Change
    {
        public string Currency { get; set; }
        public DateTime Time { get; set; }
        public double Percentage { get; set; }
        public ChangeType Type { get; set; }
        public string Message { get; set; }
    }

    public enum ChangeType
    {
        Currency, Error, Info
    }
}
