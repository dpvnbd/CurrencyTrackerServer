using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Binance;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Poloniex;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Data;
using CurrencyTrackerServer.NoticesService.Concrete;
using CurrencyTrackerServer.PriceService.Concrete.Bittrex;
using CurrencyTrackerServer.PriceService.Concrete.Poloniex;
using CurrencyTrackerServer.ReminderService;
using CurrencyTrackerServer.Web.Infrastructure.Concrete.MultipleUsers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CurrencyTrackerServer.Web.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/[controller]")]
  public class ReminderController : Controller
  {
    private readonly ReminderTimerWorker _reminder;
    private readonly PoloniexTimerWorker _pChange;
    private readonly BinanceTimerWorker _bnChange;
    private readonly BittrexTimerWorker _bChange;
    private readonly PoloniexPriceTimerWorker _pPrice;
    private readonly BittrexPriceTimerWorker _bPrice;
    private readonly DefaultNoticesTimerWorker _notices;
    private readonly UserContainersManager _userContainersManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOptions<AppSettings> _settings;

    public ReminderController(ReminderTimerWorker reminder, PoloniexTimerWorker pChange, BittrexTimerWorker bChange,
      BinanceTimerWorker bnChange,
      PoloniexPriceTimerWorker pPrice, BittrexPriceTimerWorker bPrice, DefaultNoticesTimerWorker notices,
      UserContainersManager userContainersManager, UserManager<ApplicationUser> userManager,
      IOptions<AppSettings> settings)
    {
      _reminder = reminder;
      _pChange = pChange;
      _bChange = bChange;
      _bnChange = bnChange;
      _pPrice = pPrice;
      _bPrice = bPrice;
      _notices = notices;
      _userContainersManager = userContainersManager;
      _userManager = userManager;
      _settings = settings;
    }

    [HttpPost("ping")]
    public async Task<IActionResult> Ping()
    {
      _reminder.Start();

      if (_settings.Value.BittrexChangesWorkerEnabled)
      {
        _bChange.Start();
      }

      if (_settings.Value.PoloniexChangesWorkerEnabled)
      {
        _pChange.Start();
      }

      if (_settings.Value.BinanceChangesWorkerEnabled)
      {
        _bnChange.Start();
      }

      if (_settings.Value.BittrexPriceWorkerEnabled)
      {
        _bPrice.Start();
      }

      if (_settings.Value.PoloniexPriceWorkerEnabled)
      {
        _pPrice.Start();
      }
      if (_settings.Value.PoloniexNoticesWorkerEnabled)
      {
        _notices.Start();
      }

      var user = await GetCurrentUser();
      if (user == null || !user.IsEnabled)
      {
        return BadRequest();
      }

      _userContainersManager.InitializeUserContainer(user.Id, _userManager);
      return Ok();
    }

 

    [HttpPost("start")]
    public IActionResult Start()
    {
      _reminder.Start();
      return Ok();
    }

    [HttpPost("stop")]
    public IActionResult Stop()
    {
      _reminder.Stop();
      return Ok();
    }

    [HttpGet("period")]
    public IActionResult Period()
    {
      return Ok(new PeriodSetting { Period = _reminder.Period / 1000 });
    }

    [HttpPost("period")]
    public IActionResult SetPeriod([FromBody] PeriodSetting periodBody)
    {
      if (!ModelState.IsValid) return BadRequest();
      _reminder.Period = periodBody.Period * 1000;
      return Ok();
    }

    public class PeriodSetting
    {
      [Range(3, Int32.MaxValue)]
      public int Period { get; set; }
    }

    private async Task<ApplicationUser> GetCurrentUser()
    {
      var email = User.FindFirst("sub")?.Value;
      return await _userManager.FindByEmailAsync(email);
    }
  }
}
