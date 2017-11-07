using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyTrackerServer.Infrastructure.Entities
{
    public enum UpdateDestination
    {
        None,
        CurrencyChange,
        Price,
        Reminder,
        News
    }
}
