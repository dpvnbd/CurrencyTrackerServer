﻿using System;
using System.Threading.Tasks;

namespace CurrencyTrackerServer.Infrastructure.Abstract.Data
{
    public interface IRepository<T>:IDisposable where T : class
    {
        System.Linq.IQueryable<T> GetAll();
        Task<T> Add(T entity);
        Task<T> Add(T entity, bool saveChanges);
        Task<T> Update(T entity);
        Task<T> Update(T entity, bool saveChanges);
        Task Delete(T entity);
        Task Delete(T entity, bool saveChanges);
        Task DeleteAll();
        Task SaveChanges();
    }
}