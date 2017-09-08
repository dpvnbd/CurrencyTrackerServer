using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyTrackerServer.Areas.ChangeTracker.Infrastructure;
using CurrencyTrackerServer.Areas.ChangeTracker.Models;
using CurrencyTrackerServer.ChangeTrackerService.Concrete;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Poloniex;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.Areas.ChangeTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ChangeTimerWorker _bWorker;
        private readonly ChangeTimerWorker _pWorker;


        private readonly INotifier<Change> _notifications;
        public IDataSource<string> dt;

        public HomeController(BittrexTimerWorker bWorker, PoloniexTimerWorker pWorker,
            ChangeNotificationsMessageHandler notifications)
        {
            _bWorker = bWorker;
            _pWorker = pWorker;
            _notifications = notifications;

            var settings = LoadSettings().Result;

            bWorker.ResetTimeSpan = TimeSpan.FromHours(settings.ResetHours);
            bWorker.Period = settings.Period * 1000;
            bWorker.Percentage = settings.Percentage;
            bWorker.MultipleChanges = settings.MultipleChanges;
            bWorker.MultipleChangesSpan = TimeSpan.FromMinutes(settings.MultipleChangesSpanMinutes);
            bWorker.Start();

            pWorker.ResetTimeSpan = TimeSpan.FromHours(settings.ResetHours);
            pWorker.Period = settings.Period * 1000;
            pWorker.Percentage = settings.Percentage;
            pWorker.MultipleChanges = settings.MultipleChanges;
            pWorker.MultipleChangesSpan = TimeSpan.FromMinutes(settings.MultipleChangesSpanMinutes);
            pWorker.Start();

        }


        public IActionResult Index()
        {
            var history = _bWorker.Monitor.GetHistory();
            ViewBag.WorkingStatus = new {BittrexWorking = _bWorker.Started, PoloniexWorking = _pWorker.Started};
            return View(history);
        }

        [HttpPost]
        public async Task<IActionResult> Reset()
        {
            await _bWorker.Monitor.ResetAll();
            return Ok();
        }

        [HttpPost]
        public IActionResult Start(ChangeSource source)
        {
            if (source == ChangeSource.Bittrex)
                _bWorker.Start();

            if (source == ChangeSource.Poloniex)
                _pWorker.Start();

            return Ok(new {BittrexWorking = _bWorker.Started, PoloniexWorking = _pWorker.Started});
        }

        [HttpPost]
        public IActionResult Stop(ChangeSource source)
        {
            if (source == ChangeSource.Bittrex)
                _bWorker.Stop();

            if (source == ChangeSource.Poloniex)
                _pWorker.Stop();

            return Ok(new {BittrexWorking = _bWorker.Started, PoloniexWorking = _pWorker.Started});
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
            _bWorker.Period = settings.Period;
            _bWorker.Percentage = settings.Percentage;
            _bWorker.ResetTimeSpan = TimeSpan.FromHours(settings.ResetHours);

            _bWorker.MultipleChanges = settings.MultipleChanges;
            _bWorker.MultipleChangesSpan = TimeSpan.FromMinutes(settings.MultipleChangesSpanMinutes);

            _pWorker.Period = settings.Period;
            _pWorker.Percentage = settings.Percentage;
            _pWorker.ResetTimeSpan = TimeSpan.FromHours(settings.ResetHours);

            _pWorker.MultipleChanges = settings.MultipleChanges;
            _pWorker.MultipleChangesSpan = TimeSpan.FromMinutes(settings.MultipleChangesSpanMinutes);
            try
            {
                System.IO.File.WriteAllText("settings.bittrex.json", JsonConvert.SerializeObject(settings));
            }
            catch (Exception)
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