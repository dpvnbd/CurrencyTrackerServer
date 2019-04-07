using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Data;
using CurrencyTrackerServer.Infrastructure.Entities.Price;
using CurrencyTrackerServer.PriceService.Concrete.Binance;
using CurrencyTrackerServer.PriceService.Concrete.Bittrex;
using CurrencyTrackerServer.PriceService.Concrete.Poloniex;
using CurrencyTrackerServer.Web.Infrastructure.Concrete;
using CurrencyTrackerServer.Web.Infrastructure.Concrete.MultipleUsers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CurrencyTrackerServer.Web.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/[controller]")]
  public class PriceController : Controller
  {
    private readonly ISettingsProvider _settingsProvider;
    private readonly BittrexPriceTimerWorker _bWorker;
    private readonly PoloniexPriceTimerWorker _pWorker;
    private readonly BinancePriceTimerWorker _bnWorker;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly UserContainersManager _userContainersManager;


    public PriceController(ISettingsProvider settingsProvider, BittrexPriceTimerWorker bWorker,
      PoloniexPriceTimerWorker pWorker, BinancePriceTimerWorker bnWorker, UserManager<ApplicationUser> userManager,
      UserContainersManager userContainersManager, IOptions<AppSettings> config)
    {
      _settingsProvider = settingsProvider;
      _bWorker = bWorker;
      _pWorker = pWorker;
      _bnWorker = bnWorker;
      _userManager = userManager;
      _userContainersManager = userContainersManager;

      if (config.Value.BittrexPriceWorkerEnabled)
      {
        _bWorker.Start();
      }
      if (config.Value.PoloniexPriceWorkerEnabled)
      {
        _pWorker.Start();
      }
      if (config.Value.BinancePriceWorkerEnabled)
      {
        _bnWorker.Start();
      }
    }

    [HttpGet("lastPrice/{source}/{currency}")]
    public async Task<Price> GetLastPrice(UpdateSource source, string currency)
    {
      switch (source)
      {
        case UpdateSource.Bittrex:
          return await _bWorker.GetPrice(currency);
        case UpdateSource.Poloniex:
          return await _pWorker.GetPrice(currency);
        case UpdateSource.Binance:
          return await _bnWorker.GetPrice(currency);
        default:
          return new Price { Message = "Source " + source + " doesn't exist" };
      }
    }

    [HttpPost("start/{source}")]
    public IActionResult Start(UpdateSource source)
    {
      switch (source)
      {
        case UpdateSource.Bittrex:
          _bWorker.Start();
          break;
        case UpdateSource.Poloniex:
          _pWorker.Start();
          break;
        case UpdateSource.Binance:
          _bnWorker.Start();
          break;
      }

      return Ok();
    }

    [HttpPost("stop/{source}")]
    public IActionResult Stop(UpdateSource source)
    {
      if (source == UpdateSource.Bittrex)
      {
        _bWorker.Stop();
      }
      else if (source == UpdateSource.Poloniex)
      {
        _pWorker.Stop();
      }
      else if (source == UpdateSource.Binance)
      {
        _bnWorker.Stop();
      }
      return Ok();
    }

    [HttpGet("settings/{source}")]
    public async Task<PriceSettings> Settings(UpdateSource source)
    {
      var user = await GetCurrentUser();
      return _settingsProvider.GetSettings<PriceSettings>(source, UpdateDestination.Price, user.Id);
    }

    [HttpPost("settings/{source}")]
    public async Task<IActionResult> SaveSettings(UpdateSource source, [FromBody] PriceSettings settings)
    {
      if (ModelState.IsValid)
      {
        var user = await GetCurrentUser();
        await _settingsProvider.SaveSettings(source, UpdateDestination.Price, user.Id, settings);
        return Ok();
      }
      else
      {
        return BadRequest(ModelState);
      }
    }

    [HttpPost("notification/{source}/{isEnabled}")]
    public async Task<IActionResult> SetNotification(UpdateSource source, bool isEnabled)
    {
      if (ModelState.IsValid)
      {
        var container = await GetUserContainer();
        switch (source)
        {
          case UpdateSource.Bittrex:
            container.BittrexPriceMonitor.SetNotification(isEnabled);
            break;
          case UpdateSource.Poloniex:
            container.PoloniexPriceMonitor.SetNotification(isEnabled);
            break;
          case UpdateSource.Binance:
            container.BinancePriceMonitor.SetNotification(isEnabled);
            break;
          default:
            return BadRequest();
        }
        return Ok();
      }
      else
      {
        return BadRequest(ModelState);
      }
    }

    [HttpPost("{source}")]
    public async Task<IActionResult> SetCurrencies(UpdateSource source, [FromBody] IEnumerable<Price> currencies)
    {
      if (ModelState.IsValid)
      {
        var container = await GetUserContainer();
        switch (source)
        {
          case UpdateSource.Bittrex:
            container.BittrexPriceMonitor.SetCurrencies(currencies);
            break;
          case UpdateSource.Poloniex:
            container.PoloniexPriceMonitor.SetCurrencies(currencies);
            break;
          case UpdateSource.Binance:
            container.BinancePriceMonitor.SetCurrencies(currencies);
            break;
          default:
            return BadRequest();
        }
        return Ok();
      }
      else
      {
        return BadRequest(ModelState);
      }
    }

    private async Task<UserMonitorsContainer> GetUserContainer()
    {
      var user = await GetCurrentUser();
      return (UserMonitorsContainer)_userContainersManager.GetUserContainer(user.Id, _userManager);
    }

    private async Task<ApplicationUser> GetCurrentUser()
    {
      var email = User.FindFirst("sub")?.Value;
      return await _userManager.FindByEmailAsync(email);
    }
  }
}
