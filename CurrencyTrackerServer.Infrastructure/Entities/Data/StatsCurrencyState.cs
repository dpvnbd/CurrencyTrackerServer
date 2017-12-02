using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyTrackerServer.Infrastructure.Entities.Data
{
    public class StatsCurrencyState
    {
        public UpdateSource UpdateSource { get; set; }
        public string Currency { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastChangeTime { get; set; }
        public double Percentage { get; set; }
    }
}
