using System;

namespace CurrencyTrackerServer.ChangeTrackerService.Entities
{
    public class Change
    {
        public string Currency { get; set; }
        public DateTime Time { get; set; }
        public double Percentage { get; set; }
        public double Threshold { get; set; }
        public ChangeType Type { get; set; }
        public ChangeSource ChangeSource { get; set; }
        public string Message { get; set; }

    }
}
 