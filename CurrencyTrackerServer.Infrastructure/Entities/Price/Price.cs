using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.Infrastructure.Entities.Price
{
    public class Price : BaseChangeEntity
    {
        public double Last { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public override UpdateDestination Destination => UpdateDestination.Price;
    }
}
