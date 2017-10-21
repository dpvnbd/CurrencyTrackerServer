using System;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;

namespace CurrencyTrackerServer.ReminderService
{
    public class ReminderTimerWorker : AbstractTimerWorker
    {
        private readonly INotifier<Reminder> _notifier;

        public ReminderTimerWorker(INotifier<Reminder> notifier)
        {
            _notifier = notifier;
        }

        protected override async Task DoWork()
        {
            await _notifier.SendNotificationMessage(new Reminder {Time = DateTime.Now});
        }
    }
}