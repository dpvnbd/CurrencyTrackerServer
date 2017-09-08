﻿using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Data;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete
{
   public class BittrexTimerWorker : ChangeTimerWorker
    {
        public BittrexTimerWorker(INotifier<Change> notifier, int period = 10000) : base(
            new ChangeMonitor<Repository<CurrencyStateEntity>, Repository<ChangeHistoryEntryEntity>>(
                new BittrexApiDataSource()), notifier, period)
        {
        }
    }
}