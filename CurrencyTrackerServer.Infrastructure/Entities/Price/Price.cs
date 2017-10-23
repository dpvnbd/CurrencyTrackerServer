using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.Infrastructure.Entities.Price
{
    public class Price
    {
        public ChangeSource Source { get; set; }
        public ChangeType Type { get; set; }
        public string Currency { get; set; }
        public double Last { get; set; }
        public double High { get; set; }
        public double Low { get; set; }

        public string Message { get; set; }
    }
}
