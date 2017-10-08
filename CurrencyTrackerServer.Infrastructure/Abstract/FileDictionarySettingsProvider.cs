using System;
using System.Collections.Generic;
using System.IO;
using CurrencyTrackerServer.Infrastructure.Entities;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public abstract class FileDictionarySettingsProvider<T> : ISettingsProvider<T> where T : new()
    {
        private Dictionary<ChangeSource, T> _settings;

        protected abstract string Filename { get; }

        public T GetSettings(ChangeSource key)
        {
            if (_settings == null)
            {
                LoadSettings();
            }

            if (_settings.ContainsKey(key))
            {
                return _settings[key];
            }

            _settings[key] = new T();
            SaveSettings();
            return _settings[key];
        }

        public void SaveSettings(ChangeSource key, T settings)
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
                    _settings = JsonConvert.DeserializeObject<Dictionary<ChangeSource, T>>(json);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    _settings = null;
                }
            }

            if (_settings == null)
            {
                _settings = new Dictionary<ChangeSource, T>();
            }
        }

        private void SaveSettings()
        {
            if (_settings == null)
            {
                _settings = new Dictionary<ChangeSource, T>();
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