using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using CurrencyTrackerServer.Data;
using CurrencyTrackerServer.Infrastructure.Concrete;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Services.Abstract;
using CurrencyTrackerServer.Services.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly BittrexWorker _worker;
        private readonly IBittrexService _service;
        private readonly NotificationsMessageHandler _notifications;

        public HomeController(BittrexWorker worker, IBittrexService service, NotificationsMessageHandler notifications)
        {
            var settings = LoadSettings();
            service.AutoResetHours = settings.ResetHours;
            //service.AutoResetHours = optionsAccessor.Value.ResetHours;
            _worker = worker;
            _service = service;
            _notifications = notifications;
            worker.Period = settings.Period;
            worker.ChangeOverTime = true;
            worker.ChangeInterval = TimeSpan.FromMinutes(2);
            worker.Percentage = settings.Percentage;
        }


        public IActionResult Index()
        {
            List<BittrexChange> allChanges;

            using (var repo = new BittrexHistoryRepository(new BittrexContext()))
            {
                allChanges =
                    repo.GetHistory()
                        .Where(c => c.LastNotifiedChange != null)
                        .OrderBy(c => c.LastNotifiedChange)
                        .ToList();
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
                _notifications.SendInfoMessage("Сброс информации за сутки");
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

        [HttpPost]
        public IActionResult Settings(BittrexSettingsViewModel settings)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values);
            }
            SaveSettings(settings);
            _worker.Period = settings.Period;
            _worker.Percentage = settings.Percentage;
            _service.AutoResetHours = settings.ResetHours;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ViewResult Settings()
        {
            var settings = LoadSettings();
            return View(settings);
        }

        private static BittrexSettingsViewModel LoadSettings()
        {
            string settingsJson;
            BittrexSettingsViewModel settings = null;

            if (System.IO.File.Exists("settings.bittrex.json"))
            {
                try
                {
                    settingsJson = System.IO.File.ReadAllText("settings.bittrex.json");
                }
                catch (Exception)
                {
                    settingsJson = null;
                }
                settings = JsonConvert.DeserializeObject<BittrexSettingsViewModel>(settingsJson);
            }

            if (settings == null)
            {
                settings = new BittrexSettingsViewModel();
                SaveSettings(settings);
            }

            return settings;
        }

        private static void SaveSettings(BittrexSettingsViewModel settings)
        {
            System.IO.File.WriteAllText("settings.bittrex.json", JsonConvert.SerializeObject(settings));
        }
    }
}