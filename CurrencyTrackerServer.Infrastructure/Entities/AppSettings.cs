namespace CurrencyTrackerServer.Infrastructure.Entities
{
  public class AppSettings
  {
    public int ChangeWorkerPeriodSeconds { get; set; } = 3;
    public int ChangeWorkerUpdateCycle { get; set; } = 20;
    public int ChangeMonitorStateSyncCycle { get; set; } = 10;
    public bool SaveStatesToDatabase { get; set; }
    public int PriceWorkerPeriodSeconds { get; set; } = 3;
    public int ReminderPeriodSeconds { get; set; } = 150;
    public int NoticesWorkerPeriodSeconds { get; set; } = 60;
    public int ChangesStatsPeriodHours { get; set; } = 24;

    public bool BittrexChangesWorkerEnabled { get; set; } = true;
    public bool PoloniexChangesWorkerEnabled { get; set; } = true;

    public bool BittrexPriceWorkerEnabled { get; set; } = true;
    public bool PoloniexPriceWorkerEnabled { get; set; } = true;

    public bool PoloniexNoticesWorkerEnabled { get; set; } = true;
  }
}
