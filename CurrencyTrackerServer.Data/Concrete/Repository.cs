using System;
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

    public T Add(T entity)
    {
      return Add(entity, true);
    }

    public T Update(T entity)
    {
      return Update(entity, true);
    }

    public void Delete(T entity)
    {
      Delete(entity, true);
    }

    public T Add(T entity, bool saveChanges)
    {
      if (entity == null)
      {
        throw new ArgumentNullException("entity");
      }
      Context.Add(entity);
      if (saveChanges)
      {
        Context.SaveChanges();
      }
      return entity;
    }

    public T Update(T entity, bool saveChanges)
    {
      if (entity == null)
      {
        throw new ArgumentNullException("entity");
      }
      if (saveChanges)
        Context.SaveChanges();
      return entity;
    }

    public void Delete(T entity, bool saveChanges)
    {
      if (entity == null)
      {
        throw new ArgumentNullException("entity");
      }
      _entities.Remove(entity);
      if (saveChanges)
        Context.SaveChanges();
    }


    public void DeleteAll()
    {
      foreach (var entity in _entities.AsEnumerable())
      {
        _entities.Remove(entity);
      }
      Context.SaveChanges();
    }

    public void SaveChanges()
    {
      Context.SaveChanges();
    }

    public void Dispose()
        {
            Context?.Dispose();
        }
    }
}