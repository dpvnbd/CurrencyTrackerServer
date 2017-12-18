using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Web.Infrastructure.Abstract;
using CurrencyTrackerServer.Web.Infrastructure.Concrete;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CurrencyTrackerServer.Web.Infrastructure.Websockets
{
  public class NotificationsMessageHandler : WebSocketHandler, INotifier
  {
    public NotificationsMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(
        webSocketConnectionManager)
    {
    }

    public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
    {
      var message = Encoding.UTF8.GetString(buffer);
      await SendMessageAsync(socket, message);
    }

    public void SendToAll(IEnumerable<BaseChangeEntity> changes)
    {
      if (changes.Any())
        SendMessageToAllAsync(JsonConvert.SerializeObject(changes, new JsonSerializerSettings
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver(),
        }));
    }

    public List<string> SendToConnections(IEnumerable<string> connections, IEnumerable<BaseChangeEntity> changes)
    {
      var stillConnected = new List<string>();
      var message = JsonConvert.SerializeObject(changes, new JsonSerializerSettings
      {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
      });

      foreach (var connection in connections)
      {
        var isSent = SendMessageAsync(connection, message).Result;
        if (isSent)
        {
          stillConnected.Add(connection);
        }
      }
      return stillConnected;
    }
  }
}
