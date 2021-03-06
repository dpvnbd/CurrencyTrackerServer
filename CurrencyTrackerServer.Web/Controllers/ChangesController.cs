using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Binance;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Poloniex;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Changes;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using CurrencyTrackerServer.Infrastructure.Entities.Data;
using CurrencyTrackerServer.Web.Infrastructure.Concrete.MultipleUsers;
using CurrencyTrackerServer.Web.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CurrencyTrackerServer.Web.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/[controller]")]
  public class ChangesController : Controller
  {
    private readonly BittrexTimerWorker _bWorker;
    private readonly BinanceTimerWorker _bnWorker;
    private readonly PoloniexTimerWorker _pWorker;
    private readonly ISettingsProvider _settingsProvider;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly PoloniexApiDataSource _poloniexApiDataSource;
    private readonly UserContainersManager _userContainersManager;
    private readonly IChangesStatsService<CurrencyChangeApiData> _statsService;
    private readonly IConfiguration _config;
    private readonly INotifier _notifier;

    public ChangesController(BittrexTimerWorker bWorker, PoloniexTimerWorker pWorker, BinanceTimerWorker bnWorker,
        ISettingsProvider settingsProvider, UserManager<ApplicationUser> userManager,
        PoloniexApiDataSource poloniexApiDataSource, UserContainersManager userContainersManager,
        IChangesStatsService<CurrencyChangeApiData> statsService, IOptions<AppSettings> settings, IConfiguration config,
        INotifier notifier)
    {
      _bWorker = bWorker;
      _pWorker = pWorker;
      _bnWorker = bnWorker;
      _settingsProvider = settingsProvider;
      _userManager = userManager;
      _poloniexApiDataSource = poloniexApiDataSource;
      _userContainersManager = userContainersManager;
      _statsService = statsService;
      _config = config;
      _notifier = notifier;
      if (settings.Value.BittrexChangesWorkerEnabled)
      {
        _bWorker.Start();
      }
      if (settings.Value.PoloniexChangesWorkerEnabled)
      {
        _pWorker.Start();
      }
      if (settings.Value.BinanceChangesWorkerEnabled)
      {
        _bnWorker.Start();
      }
    }

    [HttpGet("{source}")]
    public async Task<IEnumerable<Change>> GetAsync(UpdateSource source)
    {
      var container = await GetUserContainer();

      switch (source)
      {
        case UpdateSource.Bittrex:
          return await container.BittrexChangeMonitor.GetHistory();
        case UpdateSource.Poloniex:
          return await container.PoloniexChangeMonitor.GetHistory();
        case UpdateSource.Binance:
          return await container.BinanceChangeMonitor.GetHistory();
      }
      return Array.Empty<Change>();
    }

    [HttpPost("reset/{source}")]
    public async Task<IActionResult> Reset(UpdateSource source)
    {
      var container = await GetUserContainer();

      switch (source)
      {
        case UpdateSource.Bittrex:
          await container.BittrexChangeMonitor.ResetAll();
          break;
        case UpdateSource.Poloniex:
          await container.PoloniexChangeMonitor.ResetAll();
          break;
        case UpdateSource.Binance:
          await container.BinanceChangeMonitor.ResetAll();
          break;
      }
      return Ok();
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
    public async Task<ChangeSettings> Settings(UpdateSource source)
    {
      var user = await GetCurrentUser();

      return _settingsProvider.GetSettings<ChangeSettings>(source, UpdateDestination.CurrencyChange, user.Id);
    }

    [HttpPost("settings/{source}")]
    public async Task<IActionResult> SaveSettings(UpdateSource source, [FromBody] ChangeSettings settings)
    {
      if (ModelState.IsValid)
      {
        var user = await GetCurrentUser();
        await _settingsProvider.SaveSettings(source, UpdateDestination.CurrencyChange, user.Id, settings);
        return Ok();
      }
      else
      {
        return BadRequest(ModelState);
      }
    }

    [HttpGet("stats/{source}")]
    public async Task<IEnumerable<ChangePercentageDto>> Stats(UpdateSource source)
    {
      var changes = await _statsService.GetStates(source);
    
      return changes.Select(c => new ChangePercentageDto
      { Currency = c.Currency, PercentChanged = Math.Round(c.Percentage, 2) });
    }

    [HttpGet("poloniexCurrencies")]
    public async Task<IEnumerable<string>> GetPoloniexCurrencies()
    {
      var currencies = await _poloniexApiDataSource.GetCurrencies();
      return currencies;
    }

    [HttpPost("averageUpdate")]
    [AllowAnonymous]
    [EnableCors("AllowAllOrigins")]
    public async Task<IActionResult> UpdateAverageChangeStats([FromBody] AverageChangeWrapper model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest();
      }

      var key = _config["TrackerTokens:CorsSecretKey"];

      if(key != model.ApiKey)
      {
        return BadRequest();
      }
      var notification = new AverageChangeResponse
      {
        Time = model.Time,
        Percentage = model.Percentage
      };
      await _notifier.SendToAll(new[] { notification });

      return Ok();
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


    public class AverageChangeWrapper
    {
      public string ApiKey { get; set; }
      public double Percentage { get; set; }
      public DateTime Time { get; set; }
    }

    public class AverageChangeResponse: BaseChangeEntity
    {
      public double Percentage { get; set; }
      public new UpdateSource Source => UpdateSource.None;
      public new UpdateType Type => UpdateType.Stats;

    }
  }
}
