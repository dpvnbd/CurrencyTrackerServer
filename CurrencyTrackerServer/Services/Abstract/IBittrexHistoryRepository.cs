using System.Collections.Generic;
using CurrencyTrackerServer.Infrastructure.Entities;

namespace CurrencyTrackerServer.Services.Abstract
{
    public interface IBittrexHistoryRepository
    {
        void Add(BittrexChange item);
        BittrexChange Find(string currency);
        void Update(BittrexChange item);
        IEnumerable<BittrexChange> GetHistory();
        void Save();
        void ResetHistory();
        void ResetHistoryIfOlderThan(double hours);

    }
}