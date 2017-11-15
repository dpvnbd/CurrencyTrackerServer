using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Abstract.Price;
using CurrencyTrackerServer.Infrastructure.Abstract.Workers;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Price;
using Microsoft.IdentityModel.Logging;
using Serilog;

namespace CurrencyTrackerServer.PriceService.Concrete
{
  public class PriceMonitor : IMonitor<IEnumerable<BaseChangeEntity>>
  {
    private readonly IWorker<IEnumerable<ApiPrice>> _priceWorker;
    private readonly ISettingsProvider _settingsProvider;
    private readonly IMessageNotifier _messageNotifier;
    public virtual UpdateSource Source { get; protected set; }
    public UpdateDestination Destination => UpdateDestination.Price;
    public string UserId { get; }
    public event EventHandler<IEnumerable<BaseChangeEntity>> Changed;
    private object _lockObject = new object();
    private bool _processing;
    private bool _sendNotification;




    public PriceMonitor(IWorker<IEnumerable<ApiPrice>> priceWorker,
      ISettingsProvider settingsProvider, IMessageNotifier messageNotifier, string userId)
    {
      _priceWorker = priceWorker;
      _settingsProvider = settingsProvider;
      _messageNotifier = messageNotifier;
      UserId = userId;
      priceWorker.Updated += PriceWorkerOnUpdated;

      var settings = _settingsProvider.GetSettings<PriceSettings>(Source, Destination, userId);

      _sendNotification = settings.SendNotifications;
    }

    public async void SetNotification(bool value)
    {
      _sendNotification = value;
      var settings = GetSettings();
      settings.SendNotifications = value;
      await _settingsProvider.SaveSettings(Source, Destination, UserId, settings);

      var notification = new BaseChangeEntity
      {
        Source = Source,
        Destination = Destination,
        Type = UpdateType.Special,
        Special = value ? UpdateSpecial.NotificationsEnabled : UpdateSpecial.NotificationsDisabled
      };

      var message = new[] { notification };
      OnMessage(message);
    }


    public PriceSettings GetSettings()
    {
      return _settingsProvider.GetSettings<PriceSettings>(Source, Destination, UserId);
    }

    private void PriceWorkerOnUpdated(object sender, IEnumerable<ApiPrice> apiPrices)
    {

      if (_processing)
      {
        return; // Skip a step if processing takes longer than timer period
      }

      lock (_lockObject)
      {
        if (!_processing)
        {
          _processing = true;
        }
        else
        {
          return;
        }
      }

      try
      {
        var changes = CheckChanges(apiPrices);
        if (changes.Any())
        {
          
          OnMessage(changes);
          SendEmailIfChanged(changes);
        }
      }
      finally
      {
        lock (_lockObject)
        {
          _processing = false;
        }
      }
    }


    public IEnumerable<Price> CheckChanges(IEnumerable<ApiPrice> apiPrices)
    {
      var prices = new List<Price>();
      var settings = GetSettings();
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
        throw;
      }
      return prices;
    }

    private async void SendEmailIfChanged(IEnumerable<Price> prices)
    {
      if (prices == null || !prices.Any())
      {
        return;
      }

      if (!_sendNotification)
      {
        return;
      }

      var settings = GetSettings();

      if (string.IsNullOrWhiteSpace(settings.Email))
      {
        return;
      }

      var text = "";

      foreach (var price in prices)
      {
        if (price.Last > price.Low && price.Last < price.High)
        {
          continue;
        }
        text += $"{price.Source}: {price.Currency}: {price.Last:F8} ({price.Low:F8}; {price.High:F8})\n";
      }

      if (string.IsNullOrWhiteSpace(text))
      {
        return;
      }

      var isSent = await _messageNotifier.SendMessage(settings.Email, text);
      SetNotification(false);
      settings.SendNotifications = false;
      await _settingsProvider.SaveSettings(Source, Destination, UserId, settings);
    }

    private void OnMessage(IEnumerable<BaseChangeEntity> message)
    {
      var handler = Changed;
      handler?.Invoke(this, message);
    }

    public void Dispose()
    {
      _priceWorker.Updated -= PriceWorkerOnUpdated;
    }
  }
}