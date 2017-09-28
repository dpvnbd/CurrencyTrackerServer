using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Price;

namespace CurrencyTrackerServer.Infrastructure.Abstract.Price
{
    public interface IPriceMonitor<T>
    {
        PriceSettings Settings { get; }
        ChangeSource Source { get; }
        Task<IEnumerable<T>> GetPrices();
    }
}