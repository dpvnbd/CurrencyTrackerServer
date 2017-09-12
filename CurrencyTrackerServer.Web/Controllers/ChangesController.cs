using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Poloniex;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CurrencyTrackerServer.Web.Controllers
{
    [Route("api/[controller]")]
    public class ChangesController : Controller
    {
      private readonly BittrexTimerWorker _bWorker;
      private readonly PoloniexTimerWorker _pWorker;

      public ChangesController(BittrexTimerWorker bWorker, PoloniexTimerWorker pWorker)
      {
        _bWorker = bWorker;
        _pWorker = pWorker;
      }

      [HttpGet()]
      public IEnumerable<Change> Get()
      {
        return _bWorker.Monitor.GetHistory();
      }
    }
}
