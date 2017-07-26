using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyTrackerServer.BittrexService.Entities
{
    internal class BittrexApiData
    {
        public string Currency { get; set; }
        public double CurrentBid { get; set; }
        public double PreviousDayBid { get; set; }
    }
}
