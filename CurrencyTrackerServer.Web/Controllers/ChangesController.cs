using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Poloniex;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using CurrencyTrackerServer.Infrastructure.Entities.Data;
using CurrencyTrackerServer.Web.Infrastructure.Concrete.MultipleUsers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CurrencyTrackerServer.Web.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/[controller]")]
  public class ChangesController : Controller
  {
    private readonly BittrexTimerWorker _bWorker;
    private readonly PoloniexTimerWorker _pWorker;
    private readonly ISettingsProvider _settingsProvider;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly PoloniexApiDataSource _poloniexApiDataSource;
    private readonly UserContainersManager _userContainersManager;

    public ChangesController(BittrexTimerWorker bWorker, PoloniexTimerWorker pWorker,
      ISettingsProvider settingsProvider, UserManager<ApplicationUser> userManager,
      PoloniexApiDataSource poloniexApiDataSource, UserContainersManager userContainersManager)
    {
      _bWorker = bWorker;
      _pWorker = pWorker;
      _settingsProvider = settingsProvider;
      _userManager = userManager;
      _poloniexApiDataSource = poloniexApiDataSource;
      _userContainersManager = userContainersManager;

      _bWorker.Start();
      _pWorker.Start();
    }

    [HttpGet("{source}")]
    public async Task<IEnumerable<Change>> GetAsync(UpdateSource source)
    {
      var container = await GetUserContainer();

      switch (source)
      {
        case UpdateSource.Bittrex:
          return container.BittrexChangeMonitor.GetHistory();
        case UpdateSource.Poloniex:
          return container.PoloniexChangeMonitor.GetHistory();
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
        _settingsProvider.SaveSettings(source, UpdateDestination.CurrencyChange, user.Id, settings);
        return Ok();
      }
      else
      {
        return BadRequest(ModelState);
      }
    }

    [HttpGet("poloniexCurrencies")]
    public async Task<IEnumerable<string>> GetPoloniexCurrencies()
    {
      var currencies = await _poloniexApiDataSource.GetCurrencies();
      return currencies;
    }

    private async Task<UserMonitorsContainer> GetUserContainer()
    {
      var user = await GetCurrentUser();
      return (UserMonitorsContainer)await _userContainersManager.GetUserContainer(user.Id, _userManager);
    }

    private async Task<ApplicationUser> GetCurrentUser()
    {
      var name = User.FindFirst(ClaimTypes.NameIdentifier).Value;
      return await _userManager.FindByEmailAsync(name);
    }
  }
}
