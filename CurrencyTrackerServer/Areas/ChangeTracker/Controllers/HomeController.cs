using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyTrackerServer.Areas.ChangeTracker.Models;
using CurrencyTrackerServer.BittrexService.Concrete;
using CurrencyTrackerServer.BittrexService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.Areas.ChangeTracker.Controllers
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
            worker.MultipleChanges = true;
            worker.MultipleChangesSpan = TimeSpan.FromMinutes(2);
            worker.Percentage = settings.Percentage;
            worker.MultipleChanges = settings.MultipleChanges;
            worker.MultipleChangesSpan = TimeSpan.FromMinutes(settings.MultipleChangesSpanMinutes);
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
        

        public IActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Settings(SettingsViewModel settings)
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

        private async Task<SettingsViewModel> LoadSettings()
        {
            string settingsJson;
            SettingsViewModel settings = null;

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
                settings = JsonConvert.DeserializeObject<SettingsViewModel>(settingsJson);
            }

            if (settings == null)
            {
                settings = new SettingsViewModel();
                SaveSettings(settings);
            }

            return settings;
        }

        private async void SaveSettings(SettingsViewModel settings)
        {
            _worker.Period = settings.Period;
            _worker.Percentage = settings.Percentage;
            _worker.ResetTimeSpan = TimeSpan.FromHours(settings.ResetHours);

            _worker.MultipleChanges = settings.MultipleChanges;
            _worker.MultipleChangesSpan = TimeSpan.FromMinutes(settings.MultipleChangesSpanMinutes);
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