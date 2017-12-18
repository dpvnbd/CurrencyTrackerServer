using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public interface IDataSource<T>
    {
        T GetData();
        UpdateSource Source { get; }
    }
}
