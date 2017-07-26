using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public interface IChangeMonitor<TChanges>
    {
        Task<TChanges> GetChanges(int percentage, TimeSpan multipleChangesSpan, bool multipleChanges = false);
        Task<TChanges> GetHistory(int percentage, TimeSpan multipleChangesSpan, bool multipleChanges = false);
        Task ResetChanges();
    }
}
