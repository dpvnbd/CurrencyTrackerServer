using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.Infrastructure.Concrete
{
    public class NotificationsMessageHandler : WebSocketHandler
    {
        public NotificationsMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public  override Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public async void SendInfoMessage(string s)
        {
            var message = new
            {
                info = true,
                text = s
            };
            await SendMessageToAllAsync(JsonConvert.SerializeObject(message));
        }

        public async void SendNotificationMessage(IEnumerable<BittrexChange> bittrexChanges)
        {
            await SendMessageToAllAsync(JsonConvert.SerializeObject(bittrexChanges));
        }
    }
}
