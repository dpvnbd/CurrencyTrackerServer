using System.ComponentModel.DataAnnotations;

namespace CurrencyTrackerServer.Tests.ChangeTrackerService.Entities
{
    class TestEntity
    {
        [Key]
        public string Currency { get; set; }
        public double Percentage { get; set; }
    }
}
