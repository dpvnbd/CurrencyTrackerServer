using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public interface IChangeMonitor<TChanges>
    {
        Task<TChanges> GetChanges();
        TChanges GetHistory(bool allHistory = false);
        Task ResetAll();
        Task ResetStates(TimeSpan olderThan);
    }
}
