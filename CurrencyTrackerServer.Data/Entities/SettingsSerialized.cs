using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using CurrencyTrackerServer.Infrastructure.Entities.Data;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.Data.Entities
{
  public class SettingsSerialized
  {
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }

    public UpdateDestination Destination { get; set; }

    public UpdateSource Source { get; set; }

    public string SerializedSettings { get; set; }

    public T Deserialize<T>()
    {
      return JsonConvert.DeserializeObject<T>(SerializedSettings);
    }

    public void Serialize(object settings)
    {
      SerializedSettings = JsonConvert.SerializeObject(settings);
    }
  }
}

