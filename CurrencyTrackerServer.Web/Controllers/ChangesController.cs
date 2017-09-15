using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Poloniex;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CurrencyTrackerServer.Web.Controllers
{
  [Route("api/[controller]")]
  public class ChangesController : Controller
  {
    private readonly BittrexTimerWorker _bWorker;
    private readonly PoloniexTimerWorker _pWorker;

    public ChangesController(BittrexTimerWorker bWorker, PoloniexTimerWorker pWorker)
    {
      _bWorker = bWorker;
      _pWorker = pWorker;

      _bWorker.Percentage = 5;
      _bWorker.Period = 3000;
      _bWorker.ResetTimeSpan = TimeSpan.FromMinutes(15);

      
      _pWorker.ResetTimeSpan = TimeSpan.FromMinutes(15);
      _pWorker.Percentage = 5;
      _pWorker.Period = 3000;

      _bWorker.Start();
      _pWorker.Start();
    }

    [HttpGet()]
    public IEnumerable<Change> Get()
    {
      return _bWorker.Monitor.GetHistory();
    }

    [HttpPost("reset/{source}")]
    public async Task<IActionResult> Reset(ChangeSource source)
    {
      await _bWorker.Monitor.ResetFrom(source);
      return Ok();
    }

    [HttpPost("start/{source}")]
    public IActionResult Start(ChangeSource source)
    {
      if (source == ChangeSource.Bittrex)
      {
        _bWorker.Start();
      }else if (source == ChangeSource.Poloniex)
      {
        _pWorker.Start();
      }

      return Ok();
    }

    [HttpPost("stop/{source}")]
    public IActionResult Stop(ChangeSource source)
    {
      if (source == ChangeSource.Bittrex)
      {
        _bWorker.Stop();
      }
      else if (source == ChangeSource.Poloniex)
      {
        _pWorker.Stop();
      }

      return Ok();
    }
  }
}
