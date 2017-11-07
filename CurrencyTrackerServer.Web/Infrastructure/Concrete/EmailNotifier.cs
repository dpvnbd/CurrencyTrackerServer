using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;

namespace CurrencyTrackerServer.Web.Infrastructure.Concrete
{
    public class EmailNotifier:IMessageNotifier
    {
      public Task<bool> SendMessage(string address, string text)
      {
        
      }
    }
}
