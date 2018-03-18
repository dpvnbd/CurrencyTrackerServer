using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Data.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Data;
using Newtonsoft.Json;
using Serilog;

namespace CurrencyTrackerServer.Data.Concrete
{
  public class DbSettingsProvider : ISettingsProvider
  {
    private readonly IRepositoryFactory _repoFactory;

    private Dictionary<string, object> _settingsDictionary;

    public DbSettingsProvider(IRepositoryFactory repoFactory)
    {
      _repoFactory = repoFactory;
      _settingsDictionary = new Dictionary<string, object>();
    }

    public T GetSettings<T>(UpdateSource source, UpdateDestination destination, string userId) where T : new()
    {
      var key = userId + source + destination;

      if (_settingsDictionary.ContainsKey(key))
      {
        var value = _settingsDictionary[key];
        if (value != null)
        {
          return (T)value;
        }
      }
      var newSettings = GetSettingsFromDb<T>(source, destination, userId);
      _settingsDictionary[key] = newSettings;
      return newSettings;
    }


    private T GetSettingsFromDb<T>(UpdateSource source, UpdateDestination destination, string userId) where T : new()
    {
      var settings = default(T);
      using (var repo = _repoFactory.Create<SettingsSerialized>())
      {
        var dbDettings = repo.GetAll().FirstOrDefault(s =>
            (s.UserId == userId && s.Destination == destination && s.Source == source));

        if (dbDettings != null)
        {
          settings = JsonConvert.DeserializeObject<T>(dbDettings.SerializedSettings);
        }

        if (settings == null)
        {
          settings = new T();
        }
      }
      return settings;
    }

    public async Task SaveSettings<T>(UpdateSource source, UpdateDestination destination, string userId, T settings)
    {
      if (settings == null)
      {
        return;
      }

      var key = userId + source + destination;

      _settingsDictionary[key] = settings;

      using (var repo = _repoFactory.Create<SettingsSerialized>())
      {
        var dbDettings = repo.GetAll().FirstOrDefault(s =>
            (s.UserId == userId && s.Destination == destination && s.Source == source));

        var serialized = JsonConvert.SerializeObject(settings);
        if (dbDettings == null)
        {
          var newSettings = new SettingsSerialized
          {
            Source = source,
            Destination = destination,
            UserId = userId,
            SerializedSettings = serialized
          };
          await repo.Add(newSettings);
        }
        else
        {
          dbDettings.SerializedSettings = serialized;
          await repo.Update(dbDettings);
        }
      }
    }
  }
}
