using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Poloniex;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
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
    private readonly IChangeSettingsProvider _settingsProvider;

    public ChangesController(BittrexTimerWorker bWorker, PoloniexTimerWorker pWorker,
      IChangeSettingsProvider settingsProvider)
    {
      _bWorker = bWorker;
      _pWorker = pWorker;
      _settingsProvider = settingsProvider;

      _bWorker.Percentage = 5;
      _bWorker.Period = 3000;
      _bWorker.ResetTimeSpan = TimeSpan.FromMinutes(15);


      _pWorker.ResetTimeSpan = TimeSpan.FromMinutes(15);
      _pWorker.Percentage = 5;
      _pWorker.Period = 3000;

      _bWorker.Start();
      _pWorker.Start();
    }

    [HttpGet("{source}")]
    public IEnumerable<Change> GetAsync(ChangeSource source)
    {
      switch (source)
      {
        case ChangeSource.Bittrex:
          return _bWorker.Monitor.GetHistory();
        case ChangeSource.Poloniex:
          return _pWorker.Monitor.GetHistory();
      }
      return _bWorker.Monitor.GetHistory(allHistory: true);
    }

    [HttpPost("reset/{source}")]
    public async Task<IActionResult> Reset(ChangeSource source)
    {
      switch (source)
      {
        case ChangeSource.Bittrex:
          await _bWorker.Monitor.ResetAll();
          break;
        case ChangeSource.Poloniex:
          await _pWorker.Monitor.ResetAll();
          break;
      }
      return Ok();
    }

    [HttpPost("start/{source}")]
    public IActionResult Start(ChangeSource source)
    {
      switch (source)
      {
        case ChangeSource.Bittrex:
          _bWorker.Start();
          break;
        case ChangeSource.Poloniex:
          _pWorker.Start();
          break;
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

    [HttpGet("settings/{source}")]
    public ChangeSettings Settings(ChangeSource source)
    {
      return _settingsProvider.GetSettings(source);
    }

    [HttpPost("settings/{source}")]
    public IActionResult SaveSettings(ChangeSource source, [FromBody] ChangeSettings settings)
    {
      if (ModelState.IsValid)
      {
        _settingsProvider.SaveSettings(source, settings);
        return Ok();
      }
      else
      {
        return BadRequest(ModelState);
      }
    }
  }
}
