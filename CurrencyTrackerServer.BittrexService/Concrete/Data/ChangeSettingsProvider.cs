using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Data
{
    public class ChangeSettingsProvider : IChangeSettingsProvider
    {
        private Dictionary<ChangeSource, ChangeSettings> _settings;
        private const string Filename = "changeSettings.json";

        public ChangeSettings GetSettings(ChangeSource key)
        {
            if (_settings == null)
            {
                LoadSettings();
            }

            if (_settings.ContainsKey(key))
            {
                return _settings[key];
            }

            _settings[key] = new ChangeSettings();
            SaveSettings();
            return _settings[key];
        }

        public void SaveSettings(ChangeSource key, ChangeSettings settings)
        {
            if (_settings == null)
            {
                LoadSettings();
            }

            _settings[key] = settings;

            SaveSettings();
        }

        private void LoadSettings()
        {
            if (File.Exists(Filename))
            {
                try
                {
                    var json = File.ReadAllText(Filename);
                    _settings = JsonConvert.DeserializeObject<Dictionary<ChangeSource, ChangeSettings>>(json);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    _settings = null;
                }
            }

            if (_settings == null)
            {
                _settings = new Dictionary<ChangeSource, ChangeSettings>();
            }
        }

        private void SaveSettings()
        {
            if (_settings == null)
            {
                _settings = new Dictionary<ChangeSource, ChangeSettings>();
            }

            try
            {
                var json = JsonConvert.SerializeObject(_settings);
                File.WriteAllText(Filename, json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}