using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities;

namespace CurrencyTrackerServer.Services.Abstract
{
    public interface IBittrexService
    {
       IEnumerable<BittrexChange> LoadChanges();
        IEnumerable<BittrexChange> LoadChangesGreaterThan(int percentage);

        IEnumerable<BittrexChange> LoadChangesGreaterThan(int percentage, bool changeOverTime,
            TimeSpan interval);
    }
}