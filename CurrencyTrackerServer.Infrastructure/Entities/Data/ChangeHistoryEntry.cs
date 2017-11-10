using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyTrackerServer.Infrastructure.Entities.Data
{
    public class ChangeHistoryEntry
    {
        public int Id { get; set; }
        public UpdateSource UpdateSource { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        
        public ApplicationUser User { get; set; }
        public string Currency { get; set; }
        public DateTime Time { get; set; }
        public double Percentage { get; set; }
        public UpdateType Type { get; set; }
        public string Message { get; set; }
    }
}
