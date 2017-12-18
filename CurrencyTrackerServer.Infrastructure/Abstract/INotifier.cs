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
        void SendToAll(IEnumerable<BaseChangeEntity> changes);
        List<string> SendToConnections(IEnumerable<string> connections, IEnumerable<BaseChangeEntity> changes);
    }
}
