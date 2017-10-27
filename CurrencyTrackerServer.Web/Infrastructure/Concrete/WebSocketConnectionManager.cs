using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace CurrencyTrackerServer.Web.Infrastructure.Concrete
{
  public class WebSocketConnectionManager
  {
    private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

    public WebSocket GetSocketById(string id)
    {
      return _sockets.FirstOrDefault(p => p.Key == id).Value;
    }

    public ConcurrentDictionary<string, WebSocket> GetAll()
    {
      return _sockets;
    }

    public string GetId(WebSocket socket)
    {
      return _sockets.FirstOrDefault(p => p.Value == socket).Key;
    }

    public string AddSocket(WebSocket socket)
    {
      var id = CreateConnectionId();
      return _sockets.TryAdd(id, socket) ? id : null;
    }

    public async Task RemoveSocket(string id)
    {
      WebSocket socket;
      _sockets.TryRemove(id, out socket);

      try
      {
        await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
          statusDescription: "Closed by the WebSocketManager",
          cancellationToken: CancellationToken.None);
      }
      catch (Exception e)
      {
        Log.Warning(e.Message);
      }
    }

    private string CreateConnectionId()
    {
      return Guid.NewGuid().ToString();
    }
  }
}
