using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete
{
    public class ChangeTimerWorker : AbstractTimerWorker
    {
        public IChangeMonitor<IEnumerable<Change>> Monitor { get; }
        private readonly INotifier<Change> _notifier;

        public ChangeTimerWorker(IChangeMonitor<IEnumerable<Change>> monitor, INotifier<Change> notifier) : base()
        {
            Monitor = monitor;
            _notifier = notifier;
        }


        protected override async Task DoWork()
        {
            try
            {
                await Monitor.ResetStates(TimeSpan.FromHours(Monitor.Settings.ResetHours));
                var changes = await Monitor.GetChanges();
                if (changes.Any())
                    await _notifier.SendNotificationMessage(changes);
            }
            catch (Exception e)
            {
                var errorMessage = new Change
                {
                    Type = ChangeType.Error,
                    Message = e.Message,
                    Time = DateTime.Now,
                    ChangeSource = Monitor.Source
                };

                await _notifier.SendNotificationMessage(errorMessage);
            }
            finally
            {
                Period = Monitor.Settings.PeriodSeconds * 1000;
            }
        }
    }
}