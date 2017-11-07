using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Workers;
using CurrencyTrackerServer.Infrastructure.Entities;

namespace CurrencyTrackerServer.ReminderService
{
  public class ReminderTimerWorker : AbstractTimerWorker<Reminder>
  {
    private readonly INotifier _notifier;

    public ReminderTimerWorker(INotifier notifier)
    {
      _notifier = notifier;
      Period = 150 * 1000;
    }

    public override UpdateSource Source => UpdateSource.None;

    protected override async Task DoWork()
    {
      await _notifier.SendToAll(new []{new Reminder {Time = DateTime.Now}});
    }
  }
}