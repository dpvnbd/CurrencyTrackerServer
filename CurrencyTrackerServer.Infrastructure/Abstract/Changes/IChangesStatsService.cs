using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Data;

namespace CurrencyTrackerServer.Infrastructure.Abstract.Changes
{
  public interface IChangesStatsService<T>
  {
    void UpdateStates(IEnumerable<T> changes);

    IEnumerable<StatsCurrencyState> GetStates(UpdateSource source);
  }
}