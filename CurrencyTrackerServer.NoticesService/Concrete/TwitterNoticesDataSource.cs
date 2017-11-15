using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Notices;
using CurrencyTrackerServer.NoticesService.Entities;
using Microsoft.Extensions.Options;

namespace CurrencyTrackerServer.NoticesService.Concrete
{
    public class TwitterNoticesDataSource:IDataSource<IEnumerable<Notice>>
    {
        private readonly IOptions<TwitterSettings> _settings;
        private OAuth2Token _appOnly;

        public TwitterNoticesDataSource(IOptions<TwitterSettings> settings)
        {
            _settings = settings;
            Initialize();
        }

        private async void Initialize()
        {
            _appOnly = await OAuth2.GetTokenAsync(_settings.Value.ConsumerKey, _settings.Value.ConsumerSecret);
        }


        public async Task<IEnumerable<Notice>> GetData()
        {
            try
            {
                var tweets = await _appOnly.Statuses.UserTimelineAsync(screen_name: _settings.Value.UsernameToWatch, count: 10,
                        exclude_replies: true);
                return tweets.Select(t => new Notice { Time = t.CreatedAt, Message = t.Text,
                    Source = Source, Destination = UpdateDestination.Notices});
            }
            catch (Exception)
            {
                Initialize();
                throw;
            }
        }

        public virtual UpdateSource Source { get; set; }
    }
}
