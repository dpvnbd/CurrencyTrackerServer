using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public interface INotifier
    {
        Task SendToAll(IEnumerable<BaseChangeEntity> changes);
        Task<List<string>> SendToConnections(IEnumerable<string> connections, IEnumerable<BaseChangeEntity> changes);
    }
}
