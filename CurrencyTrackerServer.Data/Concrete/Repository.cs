﻿using System;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.Data.Concrete
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private DbSet<T> _entities;
        private DbContext _context;
        
        protected DbContext Context
        {
            get { return _context; }
            set
            {
                _context = value;
                _entities = Context.Set<T>();
            }
        }

        public Repository(DbContext context)
        {
            this.Context = context;
        }

        public IQueryable<T> GetAll()
        {
            return _entities.AsQueryable<T>();
        }

        public async Task<T> Add(T entity)
        {
            return await Add(entity, true);
        }

        public async Task<T> Update(T entity)
        {
            return await Update(entity, true);
        }

        public async Task Delete(T entity)
        {
            await Delete(entity, true);
        }

        public async Task<T> Add(T entity, bool saveChanges)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            await Context.AddAsync(entity);
            if (saveChanges)
            {
                await Context.SaveChangesAsync();
            }
            return entity;
        }

        public async Task<T> Update(T entity, bool saveChanges)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (saveChanges)
                await Context.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(T entity, bool saveChanges)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _entities.Remove(entity);
            if (saveChanges)
                await Context.SaveChangesAsync();
        }


        public async Task DeleteAll()
        {
            foreach (var entity in _entities.AsEnumerable())
            {
                _entities.Remove(entity);
            }
            await Context.SaveChangesAsync();
        }

        public async Task SaveChanges()
        {
            await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}