using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyTrackerServer.NoticesService.Entities
{
    public class TwitterSettings
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string UsernameToWatch { get; set; }
    }
}
