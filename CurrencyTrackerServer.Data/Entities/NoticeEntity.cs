using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Entities;

namespace CurrencyTrackerServer.Data.Entities
{
    public class NoticeEntity
    {
        public int Id { get; set; }
        public UpdateSource Source { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Message { get; set; }
    }
}
