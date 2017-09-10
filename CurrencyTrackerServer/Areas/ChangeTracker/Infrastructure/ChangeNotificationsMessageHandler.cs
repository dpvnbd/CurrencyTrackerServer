using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Concrete;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.Areas.ChangeTracker.Infrastructure
{
    public class ChangeNotificationsMessageHandler : WebSocketHandler, INotifier<Change>
    {
        public ChangeNotificationsMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(
            webSocketConnectionManager)
        {
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var message = new string(Encoding.UTF8.GetChars(buffer)).Trim('\0');
            if (message.Equals("__ping__"))
                await SendMessageAsync(socket, "__pong__");
        }

        public async Task SendNotificationMessage(IEnumerable<Change> changes)
        {
            if(changes.Any())
                await SendMessageToAllAsync(JsonConvert.SerializeObject(changes));
        }

        public async Task SendNotificationMessage(Change changes)
        {
            await SendNotificationMessage(new List<Change>() {changes});
        }
    }
}