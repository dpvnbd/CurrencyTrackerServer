using System;
using System.ComponentModel.DataAnnotations;

namespace CurrencyTrackerServer.ChangeTrackerService.Entities
{
    public class ChangeHistoryEntryEntity
    {
        public int Id { get; set; }

        public ChangeSource ChangeSource { get; set; }


        public string Currency { get; set; }
        public DateTime Time { get; set; }
        public double Percentage { get; set; }
        public ChangeType Type { get; set; }
        public string Message { get; set; }
    }
}
