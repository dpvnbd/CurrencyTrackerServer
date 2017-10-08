namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public interface IRepositoryFactory
    {
        IRepository<T> Create<T>() where T : class;
    }
}