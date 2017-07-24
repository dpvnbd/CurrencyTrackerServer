using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.Data;
using CurrencyTrackerServer.Infrastructure.Concrete;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Services.Concrete;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly BittrexWorker _worker;
        private readonly NotificationsMessageHandler _notificationsMessageHandler;

        public HomeController(BittrexWorker worker, NotificationsMessageHandler notificationsMessageHandler)
        {
            _worker = worker;
            _worker.Period = 3000;
            _worker.Percentage = 3;
            _worker.ChangeOverTime = false;
            _worker.Message += WorkerOnMessage;
            _worker.InfoMessage += WorkerOnInfoMessage;

            _worker.Enabled = true;

            _notificationsMessageHandler = notificationsMessageHandler;
            
        }

        private  void WorkerOnInfoMessage(object sender, string s)
        {
            var message = new
            {
                info = true,
                text = s
            };
            _notificationsMessageHandler.SendMessageToAllAsync(JsonConvert.SerializeObject(message));
        }

        private  void WorkerOnMessage(object sender, IEnumerable<BittrexChange> bittrexChanges)
        {
             _notificationsMessageHandler.SendMessageToAllAsync(JsonConvert.SerializeObject(bittrexChanges));
        }

        public IActionResult Index()
        {
            List<BittrexChange> allChanges;

            using (var repo = new BittrexHistoryRepository(new BittrexContext()))
            {
                allChanges = repo.GetHistory().Where(c=>c.LastNotifiedChange != null).OrderBy(c=> c.LastNotifiedChange).ToList();
            }
            object json = JsonConvert.SerializeObject(allChanges);
            return View(json);
        }

        [HttpPost]
        public IActionResult Reset()
        {
            using (var repo = new BittrexHistoryRepository(new BittrexContext()))
            {
                repo.ResetHistory();
                repo.Save();
            }
            return Ok();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        
    }
}
