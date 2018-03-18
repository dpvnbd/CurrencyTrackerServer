using CurrencyTrackerServer.Infrastructure.Abstract.Data;

namespace CurrencyTrackerServer.Infrastructure.Abstract.Data
{
    public interface IRepositoryFactory
    {
        IRepository<T> Create<T>() where T : class;
    }
}