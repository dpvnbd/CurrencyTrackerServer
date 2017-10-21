using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CurrencyTrackerServer.ReminderService;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyTrackerServer.Web.Controllers
{
  [Route("api/[controller]")]
  public class ReminderController : Controller
  {
    private readonly ReminderTimerWorker _reminder;

    public ReminderController(ReminderTimerWorker reminder)
    {
      _reminder = reminder;
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
      return Ok(new PeriodSetting{ Period = _reminder.Period / 1000});
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
