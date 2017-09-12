using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Web.Infrastructure.Abstract;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.Web.Infrastructure.Concrete
{
    public class ChangeNotificationsMessageHandler : WebSocketHandler, INotifier<Change>
    {
        public ChangeNotificationsMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(
            webSocketConnectionManager)
        {
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
          await SendMessageAsync(socket, Encoding.UTF8.GetString(buffer));
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
