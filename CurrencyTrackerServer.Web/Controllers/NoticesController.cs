using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities.Notices;
using CurrencyTrackerServer.NoticesService.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyTrackerServer.Web.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/[controller]")]
  public class NoticesController : Controller
  {
    private readonly PoloniexNoticesMonitor _monitor;
    private readonly DefaultNoticesTimerWorker _timerWorker;

    public NoticesController(PoloniexNoticesMonitor monitor, DefaultNoticesTimerWorker timerWorker)
    {
      _monitor = monitor;
      _timerWorker = timerWorker;
      _timerWorker.Start();
    }

    // GET
    public IEnumerable<Notice> Index()
    {
      return _monitor.GetNotices();
    }
  }
}
