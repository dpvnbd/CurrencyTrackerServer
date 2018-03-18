using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public interface IDataSource<T>
    {
        Task<T> GetData();
        UpdateSource Source { get; }
    }
}
