using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public interface IMessageNotifier
    {
        Task<bool> SendMessage(string address, string text);
    }
}
