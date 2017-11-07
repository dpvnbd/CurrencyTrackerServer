using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Poloniex;
using CurrencyTrackerServer.Infrastructure.Entities.Data;
using CurrencyTrackerServer.PriceService.Concrete.Bittrex;
using CurrencyTrackerServer.PriceService.Concrete.Poloniex;
using CurrencyTrackerServer.ReminderService;
using CurrencyTrackerServer.Web.Infrastructure.Concrete.MultipleUsers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyTrackerServer.Web.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/[controller]")]
  public class ReminderController : Controller
  {
    private readonly ReminderTimerWorker _reminder;
    private readonly PoloniexTimerWorker _pChange;
    private readonly BittrexTimerWorker _bChange;
    private readonly PoloniexPriceTimerWorker _pPrice;
    private readonly BittrexPriceTimerWorker _bPrice;
    private readonly UserContainersManager _userContainersManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReminderController(ReminderTimerWorker reminder, PoloniexTimerWorker pChange, BittrexTimerWorker bChange,
      PoloniexPriceTimerWorker pPrice, BittrexPriceTimerWorker bPrice, UserContainersManager userContainersManager,
      UserManager<ApplicationUser> userManager)
    {
      _reminder = reminder;
      _pChange = pChange;
      _bChange = bChange;
      _pPrice = pPrice;
      _bPrice = bPrice;
      _userContainersManager = userContainersManager;
      _userManager = userManager;
    }

    [HttpPost("ping")]
    public async Task<IActionResult> Ping()
    {
      _reminder.Start();
      _pChange.Start();
      _bChange.Start();
      _pPrice.Start();
      _bPrice.Start();

      var user = await GetCurrentUser();
      if (user == null)
      {
        return BadRequest();
      }

      await _userContainersManager.InitializeUserContainer(user.Id, _userManager);
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
      return Ok(new PeriodSetting {Period = _reminder.Period / 1000});
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
      var name = User.FindFirst(ClaimTypes.NameIdentifier).Value;
      return await _userManager.FindByEmailAsync(name);
    }
  }
}
