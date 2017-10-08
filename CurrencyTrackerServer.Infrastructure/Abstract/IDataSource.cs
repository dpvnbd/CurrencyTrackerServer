using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.Infrastructure.Abstract
{
    public interface IDataSource<T>
    {
        Task<T> GetData();
        ChangeSource Source { get; }
    }
}
