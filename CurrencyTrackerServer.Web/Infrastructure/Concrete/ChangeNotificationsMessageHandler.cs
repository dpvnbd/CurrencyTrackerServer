using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using CurrencyTrackerServer.Web.Infrastructure.Abstract;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            var message = Encoding.UTF8.GetString(buffer);
            await SendMessageAsync(socket, message);
        }

        public async Task SendNotificationMessage(IEnumerable<Change> changes)
        {
            if (changes.Any())
                await SendMessageToAllAsync(JsonConvert.SerializeObject(changes, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
        }

        public async Task SendNotificationMessage(Change changes)
        {
            await SendNotificationMessage(new List<Change>() { changes });
        }
    }
}
