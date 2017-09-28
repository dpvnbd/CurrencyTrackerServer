using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public interface INotifier<in T>
    {
        Task SendNotificationMessage(T message);
        Task SendNotificationMessage(IEnumerable<T> message);
    }
}
