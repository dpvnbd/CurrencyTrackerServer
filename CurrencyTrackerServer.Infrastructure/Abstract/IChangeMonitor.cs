using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public interface IChangeMonitor<TChanges>
    {
        Task<TChanges> GetChanges(double percentage, TimeSpan multipleChangesSpan, bool multipleChanges = false);
        TChanges GetHistory();
        Task ResetAll();
        Task ResetStates(TimeSpan olderThan);
    }
}
