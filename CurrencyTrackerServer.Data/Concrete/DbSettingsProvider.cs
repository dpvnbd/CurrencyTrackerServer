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

namespace CurrencyTrackerServer.Data.Concrete
{
    public class DbSettingsProvider : ISettingsProvider
    {
        private readonly IRepositoryFactory _repoFactory;

        public DbSettingsProvider(IRepositoryFactory repoFactory)
        {
            _repoFactory = repoFactory;
        }
        public T GetSettings<T>(UpdateSource source, UpdateDestination destination, string userId) where T : new()
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

    public void SaveSettings<T>(UpdateSource source, UpdateDestination destination, string userId, T settings)
    {

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
          repo.Add(newSettings);
        }
        else
        {
          dbDettings.SerializedSettings = serialized;
          repo.Update(dbDettings);
        }
      }
    }
  }
}
