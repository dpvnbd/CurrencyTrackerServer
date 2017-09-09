using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CurrencyTrackerServer.Areas.ChangeTracker.Models
{
    public class SettingsViewModel
    {
        [DisplayName("Период обновления, сек"), Range(3, int.MaxValue, ErrorMessage = "Минимум 3 секунды")]
        public int Period { get; set; } = 5;

        [DisplayName("Изменение, %")]
        public int Percentage { get; set; } = 3;

        [DisplayName("Сброс, часов")]
        public double ResetHours { get; set; } = 24;

        [DisplayName("Учитывать два и больше изменения за время")]
        public bool MultipleChanges { get; set; } = true;

        [DisplayName("Время, минут")]
        public int MultipleChangesSpanMinutes { get; set; } = 2;
    }
}
