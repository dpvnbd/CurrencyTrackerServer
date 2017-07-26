using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.BittrexService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.BittrexService.Concrete
{
    public class BittrexChangeMonitor<TStateRepo, THistoryRepo> : IChangeMonitor<Change> 
        where TStateRepo : IRepository<CurrencyStateEntity>, new() where THistoryRepo : IRepository<ChangeHistoryEntryEntity>, new()

    {
        public Task<Change> GetChanges(int percentage, TimeSpan multipleChangesSpan, bool multipleChanges = false)
        {
            throw new NotImplementedException();
        }

        public Task<Change> GetHistory(int percentage, TimeSpan multipleChangesSpan, bool multipleChanges = false)
        {
            throw new NotImplementedException();
        }

        public async Task ResetChanges()
        {
            using (var repo = new TStateRepo())
            {
                await repo.DeleteAll();
            }

            using (var repo = new THistoryRepo())
            {
                var entry = new ChangeHistoryEntryEntity
                {
                    Type = HistoryEntryType.Info,
                    Message = "Сброс информации о валютах",
                    Time = DateTime.Now
                };

                await repo.Add(entry);
            }
        }
    }
}