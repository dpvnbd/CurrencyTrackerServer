using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Data
{
    public class RepositoryFactory : IRepositoryFactory
    {
        public IDesignTimeDbContextFactory<DbContext> ContextFactory { get; }

        public RepositoryFactory(IDesignTimeDbContextFactory<DbContext> contextFactory)
        {
            this.ContextFactory = contextFactory;
        }

        public IRepository<T> Create<T>() where T : class
        {
            return new Repository<T>(ContextFactory.CreateDbContext(new string[] {}));
        }
    }
}