using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Poloniex;
using CurrencyTrackerServer.PriceService.Concrete.Bittrex;
using CurrencyTrackerServer.PriceService.Concrete.Poloniex;
using CurrencyTrackerServer.ReminderService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

    public ReminderController(ReminderTimerWorker reminder, PoloniexTimerWorker pChange, BittrexTimerWorker bChange,
      PoloniexPriceTimerWorker pPrice, BittrexPriceTimerWorker bPrice)
    {
      _reminder = reminder;
      _pChange = pChange;
      _bChange = bChange;
      _pPrice = pPrice;
      _bPrice = bPrice;
    }

    [HttpPost("ping")]
    public IActionResult Ping()
    {
      _reminder.Start();
      _pChange.Start();
      _bChange.Start();
      _pPrice.Start();
      _bPrice.Start();
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
  }
}
