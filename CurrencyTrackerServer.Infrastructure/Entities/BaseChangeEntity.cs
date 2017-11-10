using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.Infrastructure.Entities
{
    public class BaseChangeEntity
    {


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Currency { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Time { get; set; }

        public UpdateType Type { get; set; }
        public UpdateSource Source { get; set; }
        public virtual UpdateDestination Destination { get; set; }

        /// <summary>
        /// Used for special notifications if Type == UpdateType.Special
        /// </summary>
        public UpdateSpecial Special { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
    }
}
