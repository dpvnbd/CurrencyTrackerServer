using System.Collections;
using System.Collections.Generic;

namespace CurrencyTrackerServer.Infrastructure.Abstract.Price
{
    public interface IPriceMonitor<out T>
    {
        IEnumerable<string> Currencies { get; set; }
        IEnumerable<T> GetPrices();
    }
}