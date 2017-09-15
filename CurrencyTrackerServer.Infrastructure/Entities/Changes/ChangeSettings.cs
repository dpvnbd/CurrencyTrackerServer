﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CurrencyTrackerServer.Infrastructure.Entities.Changes
{
    public class ChangeSettings
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Период не меньше секунды")]
        public int PeriodSeconds { get; set; } = 3;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Минимум 1%")]
        public int Percentage { get; set; } = 3;

        [Required]
        [Range(0.1, Double.MaxValue)]
        public double ResetHours { get; set; } = 24;


        public bool MultipleChanges { get; set; } = true;
        public int MultipleChangesSpanMinutes { get; set; } = 1;


        public int MarginPercentage { get; set; } = 1;
        public List<string> MarginCurrencies;
    }
}