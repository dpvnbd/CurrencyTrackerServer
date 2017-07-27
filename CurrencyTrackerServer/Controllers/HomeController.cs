using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using CurrencyTrackerServer.BittrexService.Concrete;
using CurrencyTrackerServer.BittrexService.Entities;
using CurrencyTrackerServer.Infrastructure.Concrete;
using CurrencyTrackerServer.Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using CurrencyTrackerServer.Infrastructure.Abstract;

namespace CurrencyTrackerServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly BittrexTimerWorker _worker;

        private readonly IChangeMonitor<List<Change>> _service;

        private readonly INotifier<Change> _notifications;
        public IDataSource<string> dt;

        public HomeController(BittrexTimerWorker worker, IChangeMonitor<List<Change>> service,
            INotifier<Change> notifications)
        {
            _worker = worker;
            _service = service;
            _notifications = notifications;

            var settings = LoadSettings().Result;
            worker.ResetTimeSpan = TimeSpan.FromHours(settings.ResetHours);
            worker.Period = settings.Period;
            worker.MultipleChanges = false;
            worker.MultipleChangesSpan = TimeSpan.FromMinutes(2);
            worker.Percentage = settings.Percentage;
            worker.Enabled = true;
        }


        public IActionResult Index()
        {

            var history = _service.GetHistory();
            return View(history);
        }

        [HttpPost]
        public async Task<IActionResult> Reset()
        {
            await _service.ResetAll();
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
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ViewResult> Settings()
        {
            var settings = await LoadSettings();
            return View(settings);
        }

        private async Task<BittrexSettingsViewModel> LoadSettings()
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
                    await _notifications.SendNotificationMessage(new Change
                    {
                        Type = ChangeType.Error,
                        Message = "Ошибка загрузки настроек",
                        Time = DateTime.Now
                    });
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

        private async void SaveSettings(BittrexSettingsViewModel settings)
        {
            _worker.Period = settings.Period;
            _worker.Percentage = settings.Percentage;
            _worker.ResetTimeSpan = TimeSpan.FromHours(settings.ResetHours);

            try
            {
                System.IO.File.WriteAllText("settings.bittrex.json", JsonConvert.SerializeObject(settings));
            }
            catch (Exception e)
            {
                await _notifications.SendNotificationMessage(new Change
                {
                    Type = ChangeType.Error,
                    Message = "Ошибка сохранения настроек",
                    Time = DateTime.Now
                });
            }
        }
    }
}