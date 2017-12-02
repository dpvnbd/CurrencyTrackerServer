namespace CurrencyTrackerServer.Infrastructure.Entities
{
  public class AppSettings
  {
    public int ChangeWorkerPeriodSeconds { get; set; } = 3;
    public int ChangeWorkerUpdateCycle { get; set; } = 20;
    public int PriceWorkerPeriodSeconds { get; set; } = 3;
    public int ReminderPeriodSeconds { get; set; } = 150;
    public int NoticesWorkerPeriodSeconds { get; set; } = 60;
    public int ChangesStatsPeriodHours { get; set; } = 24;
  }
}
