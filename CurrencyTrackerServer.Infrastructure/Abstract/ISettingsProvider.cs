using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public interface ISettingsProvider<T>
    {
        T GetSettings(ChangeSource key);
        void SaveSettings(ChangeSource key, T settings);
    }
}