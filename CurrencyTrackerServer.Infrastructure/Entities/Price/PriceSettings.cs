using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CurrencyTrackerServer.Infrastructure.Entities.Price
{
    public class PriceSettings
    {
        private List<Price> _currencies;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Период не меньше секунды")]
        public int PeriodSeconds { get; set; } = 3;

        public List<Price> Currencies
        {
            get => _currencies ?? (_currencies = new List<Price>());
            set => _currencies = value;
        }
    }
}