using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CurrencyTrackerServer.Infrastructure.Entities.Price
{
  public class PriceSettings
  {
    private List<Price> _currencies;


    public List<Price> Prices
    {
      get => _currencies ?? (_currencies = new List<Price>());
      set => _currencies = value; 
    }

    public bool SendNotifications { get; set; }

    [EmailAddress]
    public string Email { get; set; }
  }
}