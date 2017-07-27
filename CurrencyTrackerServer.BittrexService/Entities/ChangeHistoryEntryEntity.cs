using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CurrencyTrackerServer.BittrexService.Entities
{
    public class ChangeHistoryEntryEntity
    {
        [Key]
        public int Id { get; set; }
        
        public string Currency { get; set; }
        public DateTime Time { get; set; }
        public double Percentage { get; set; }
        public ChangeType Type { get; set; }
        public string Message { get; set; }
    }
}
