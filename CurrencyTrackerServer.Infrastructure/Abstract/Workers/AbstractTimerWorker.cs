using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities;
using Serilog;

namespace CurrencyTrackerServer.Infrastructure.Abstract.Workers
{
  public abstract class AbstractTimerWorker<TUpdate> : IWorker<TUpdate>
  {
    public abstract UpdateSource Source { get; }

    private readonly Timer _timer;
    public bool Started { get; private set; }
    public int Period { get; set; }
    private object _lockObject = new object();


    protected AbstractTimerWorker(int period = 3000)
    {
      Period = period;
      _timer = new Timer(TimerTick, _timer, Timeout.Infinite, Timeout.Infinite);
    }

    public void Start()
    {
      if (!Started)
      {
        lock (_lockObject)
        {
          if (!Started) //double check after lock to be thread safe
          {
            Started = true;
            _timer.Change(Period, Timeout.Infinite);
          }
        }
      }
    }

    public void Stop()
    {
      lock (_lockObject)
      {
        Started = false;
      }
    }

    private void TimerTick(object state)
    {
      try
      {
        DoWork();
      }
      catch (Exception e)
      {
        Log.Error(e, "Error while checking changes");
      }
      
      if (Started)
      {
        _timer.Change(Period, Timeout.Infinite);
      }
    }

    /// <summary>
    /// Method is called periodically by timer.
    /// If updates are found, it should invoke the event Updated.
    /// </summary>
    /// <returns></returns>
    protected abstract void DoWork();


    /// <summary>
    /// All monitors should subscribe to receive updates
    /// </summary>
    public event EventHandler<TUpdate> Updated;

    protected void OnUpdated(TUpdate e)
    {
      // Note the copy to a local variable, so that we don't risk a
      // NullReferenceException if another thread unsubscribes between the test and
      // the invocation.
      var handler = Updated;
      handler?.Invoke(this, e);
    }
  }
}