using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CurrencyTrackerServer.Tests.BittrexService.Entities
{
    class TestEntity
    {
        [Key]
        public string Currency { get; set; }
        public double Percentage { get; set; }
    }
}
