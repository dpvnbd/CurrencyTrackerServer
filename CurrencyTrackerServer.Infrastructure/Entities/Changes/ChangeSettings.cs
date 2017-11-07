using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CurrencyTrackerServer.Infrastructure.Entities.Changes
{
    public class ChangeSettings
    {
        [Required]
        [Range(0.1, double.MaxValue)]
        public double Percentage { get; set; } = 3;
                
        [NotMapped]
        public double ResetHours { get; set; } = 24;

        public bool MultipleChanges { get; set; } = true;
        public int MultipleChangesSpanMinutes { get; set; } = 1;


        public double MarginPercentage { get; set; } = 0.5;
        public List<string> MarginCurrencies;
    }
}