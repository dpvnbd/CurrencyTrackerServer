using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.Infrastructure.Abstract.Data
{
  public interface ISettingsProvider
  {
    T GetSettings<T>(UpdateSource source, UpdateDestination destination, string userId) where T : new();
    Task SaveSettings<T>(UpdateSource source, UpdateDestination destination, string userId, T settings);
  }
}