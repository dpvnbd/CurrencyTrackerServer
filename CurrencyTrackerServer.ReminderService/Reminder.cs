using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Entities;

namespace CurrencyTrackerServer.ReminderService
{
    public class Reminder:BaseChangeEntity
    {
        public override UpdateDestination Destination => UpdateDestination.Reminder;
    }
}
