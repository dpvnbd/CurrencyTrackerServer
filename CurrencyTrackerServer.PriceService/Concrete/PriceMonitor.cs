using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Abstract.Price;
using CurrencyTrackerServer.Infrastructure.Abstract.Workers;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Price;

namespace CurrencyTrackerServer.PriceService.Concrete
{
  public class PriceMonitor : IMonitor<IEnumerable<BaseChangeEntity>>
  {
    private readonly ISettingsProvider _settingsProvider;
    public virtual UpdateSource Source { get; protected set; }
    public UpdateDestination Destination => UpdateDestination.Price;
    public string UserId { get; }
    public event EventHandler<IEnumerable<BaseChangeEntity>> Changed;

    public PriceSettings Settings => _settingsProvider.GetSettings<PriceSettings>(Source, Destination, UserId);

    public PriceMonitor(IWorker<IEnumerable<ApiPrice>> priceWorker,
      ISettingsProvider settingsProvider, string userId)
    {
      _settingsProvider = settingsProvider;
      UserId = userId;
      priceWorker.Updated += PriceWorkerOnUpdated;
    }

    private void PriceWorkerOnUpdated(object sender, IEnumerable<ApiPrice> apiPrices)
    {
      var changes = CheckChanges(apiPrices);
      if (changes.Any())
      {
        var handler = Changed;
        handler?.Invoke(this, changes);
      }
    }


    public IEnumerable<Price> CheckChanges(IEnumerable<ApiPrice> apiPrices)
    {
      var prices = new List<Price>();
      var settings = Settings;
      try
      {
        foreach (var apiPrice in apiPrices)
        {
          var price = settings.Prices.SingleOrDefault(c => c.Currency == apiPrice.Currency);
          if (price == null)
          {
            continue;
          }
          price.Last = apiPrice.Last;
          price.Source = apiPrice.Source;
          price.Type = UpdateType.Currency;
          prices.Add(price);
        }
      }
      catch (Exception e)
      {
        prices.Add(new Price { Type = UpdateType.Error, Source = Source, Message = e.Message });
      }
      return prices;
    }

    private void SendEmailIfChanged(IEnumerable<Price> prices)
    {
      if (prices == null || !prices.Any())
      {
        return;
      }

      var changedList = new List<Price>();

      foreach (var price in prices)
      {
        if (price.Last > price.Low && price.Last < price.High)
        {
          continue;
        }
        changedList.Add(price);
      }

      if (!changedList.Any())
      {
        return;
      }


    }
  }
}