using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public interface IRepository<T>:IDisposable where T : class
    {
        IEnumerable<T> GetAll();
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task Delete(T entity);
        Task DeleteAll();
    }
}