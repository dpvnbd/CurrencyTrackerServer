using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public interface IRepository<T>:IDisposable where T : class
    {
        System.Linq.IQueryable<T> GetAll();
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task Delete(T entity);
        Task DeleteAll();
    }
}