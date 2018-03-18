using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using Microsoft.EntityFrameworkCore.Design;

namespace CurrencyTrackerServer.Data.Concrete
{
    public class RepositoryFactory : IRepositoryFactory
    {
        public IDesignTimeDbContextFactory<AppDbContext> ContextFactory { get; }

        public RepositoryFactory(IDesignTimeDbContextFactory<AppDbContext> contextFactory)
        {
            this.ContextFactory = contextFactory;
        }

        public IRepository<T> Create<T>() where T : class
        {
            return new Repository<T>(ContextFactory.CreateDbContext(new string[] {}));
        }
    }
}