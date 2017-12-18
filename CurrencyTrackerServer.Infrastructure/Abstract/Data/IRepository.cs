using System;

namespace CurrencyTrackerServer.Infrastructure.Abstract.Data
{
    public interface IRepository<T>:IDisposable where T : class
    {
        System.Linq.IQueryable<T> GetAll();
        T Add(T entity);
        T Add(T entity, bool saveChanges);
        T Update(T entity);
        T Update(T entity, bool saveChanges);
        void Delete(T entity);
        void Delete(T entity, bool saveChanges);
        void DeleteAll();
        void SaveChanges();
    }
}