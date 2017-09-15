using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Data
{
    public class RepositoryFactory
    {
        private readonly DbContextFactoryOptions _options;
        public IDbContextFactory<DbContext> ContextFactory { get; }

        public RepositoryFactory(IDbContextFactory<DbContext> contextFactory, DbContextFactoryOptions options)
        {
            _options = options;
            this.ContextFactory = contextFactory;
        }

        public IRepository<T> Create<T>() where T : class
        {
            return new Repository<T>(ContextFactory.Create(_options));
        }
    }
}
