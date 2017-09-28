using System.Collections.Generic;
using CurrencyTrackerServer.Infrastructure.Abstract.Price;
using CurrencyTrackerServer.Infrastructure.Entities.Price;

namespace CurrencyTrackerServer.PriceService.Concrete
{
    class PriceMonitor:IPriceMonitor<Price>
    {
        public List<string> Currencies { get; set; }

        public IEnumerable<Price> GetPrices()
        {
            throw new System.NotImplementedException();
        }
    }
}
