using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Notices;
using CurrencyTrackerServer.NoticesService.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CurrencyTrackerServer.Web.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/[controller]")]
  public class NoticesController : Controller
  {
    private readonly PoloniexNoticesMonitor _monitor;
    private readonly DefaultNoticesTimerWorker _noticesTimerWorker;

    public NoticesController(PoloniexNoticesMonitor monitor,
      DefaultNoticesTimerWorker noticesTimerWorker, IOptions<AppSettings> config)
    {
      _monitor = monitor;
      _noticesTimerWorker = noticesTimerWorker;
      if (config.Value.PoloniexNoticesWorkerEnabled)
      {
        _noticesTimerWorker.Start();
      }
    }

    // GET
    public IEnumerable<Notice> Index()
    {
      return _monitor.GetNotices();
    }
  }
}
