using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Entities;

namespace CurrencyTrackerServer.Infrastructure.Abstract.Workers
{
    public abstract class AbstractUserMonitorsContainer
    {
        public abstract string UserToken { get; set; }
        public abstract IList<IMonitor<IEnumerable<BaseChangeEntity>>> Monitors { get; }
        public ChangedCallbackDelegate ChangedCallback;
        public delegate void ChangedCallbackDelegate(string userToken, IEnumerable<BaseChangeEntity> changes);
    }
}
