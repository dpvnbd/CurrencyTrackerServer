using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyTrackerServer.Infrastructure.Entities.Data
{
    public class StatsAverageChangeEntry
  {
        public UpdateSource UpdateSource { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public double Percentage { get; set; }
    }
}
