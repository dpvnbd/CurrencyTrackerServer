using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities.Price;

namespace CurrencyTrackerServer.PriceService.Concrete
{
    public class PriceSettingsProvider:FileDictionarySettingsProvider<PriceSettings>
    {
        protected override string Filename => "currencies.settings.json";
    }
}
