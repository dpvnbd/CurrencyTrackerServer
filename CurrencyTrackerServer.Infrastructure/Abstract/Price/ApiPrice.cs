using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.Infrastructure.Abstract.Price
{
    public class ApiPrice
    {
        public ChangeSource Source { get; set; }

        public string Currency { get; set; }
        public double Last { get; set; }
    }
}
