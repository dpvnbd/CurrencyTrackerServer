using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public interface IChangeSettingsProvider
    {
        ChangeSettings GetSettings(ChangeSource key);
        void SaveSettings(ChangeSource key, ChangeSettings settings);
    }
}