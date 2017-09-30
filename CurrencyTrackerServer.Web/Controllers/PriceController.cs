using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Price;
using CurrencyTrackerServer.PriceService.Concrete.Bittrex;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CurrencyTrackerServer.Web.Controllers
{
  [Route("api/[controller]")]
  public class PriceController : Controller
  {
    private readonly ISettingsProvider<PriceSettings> _settingsProvider;
    private readonly BittrexPriceTimerWorker _bWorker;


    public PriceController(ISettingsProvider<PriceSettings> settingsProvider, BittrexPriceTimerWorker bWorker)
    {
      _settingsProvider = settingsProvider;
      _bWorker = bWorker;
      _bWorker.Start();
    }

    [HttpGet("{source}")]
    public IEnumerable<Price> Get(ChangeSource source)
    {
      switch (source)
      {
        case ChangeSource.Bittrex:
          return _bWorker.Monitor.Settings.Prices;
        case ChangeSource.Poloniex:
          return _bWorker.Monitor.Settings.Prices; //TODO: Change to Poloniex
      }
      return new Price[0];
    }


    [HttpGet("lastPrice/{source}/{currency}")]
    public async Task<Price> GetLastPrice(ChangeSource source, string currency)
    {
      switch (source)
      {
        case ChangeSource.Bittrex:
          return await _bWorker.Monitor.GetPrice(currency);
        case ChangeSource.Poloniex:
          return await _bWorker.Monitor.GetPrice(currency); //TODO: Change to Poloniex
        default:
          return new Price {Message = "Source " + source + " doesn't exist"};
      }
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
          _bWorker.Start(); //TODO: Change to Poloniex
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
        _bWorker.Stop(); //TODO: Change to Poloniex
      }

      return Ok();
    }

    [HttpGet("settings/{source}")]
    public PriceSettings Settings(ChangeSource source)
    {
      return _settingsProvider.GetSettings(source);
    }

    [HttpPost("settings/{source}")]
    public IActionResult SaveSettings(ChangeSource source, [FromBody] PriceSettings settings)
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
