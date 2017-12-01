using System;

namespace CurrencyTrackerServer.Infrastructure.Entities.Changes
{
    public class Change : BaseChangeEntity
    {
        public double Percentage { get; set; }
        public double Threshold { get; set; }
        public override UpdateDestination Destination => UpdateDestination.CurrencyChange;

        public bool IsSmaller {get;set;}
    }
}
