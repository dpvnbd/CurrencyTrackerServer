using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using CurrencyTrackerServer.BittrexService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.Infrastructure.Concrete
{
    public class NotificationsMessageHandler : WebSocketHandler
    {
        public NotificationsMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(
            webSocketConnectionManager)
        {
        }

        public override Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public override async Task SendNotificationMessage(IEnumerable<Change> changes)
        {
            if(changes.Any())
                await SendMessageToAllAsync(JsonConvert.SerializeObject(changes));
        }

        public override async Task SendNotificationMessage(Change changes)
        {
            await SendNotificationMessage(new List<Change>() {changes});
        }
    }
}