using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CurrencyTrackerServer.Web.Infrastructure.Concrete;

namespace CurrencyTrackerServer.Web.Infrastructure.Abstract
{
  public abstract class WebSocketHandler
  {
    protected WebSocketConnectionManager WebSocketConnectionManager { get; set; }

    public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager)
    {
      WebSocketConnectionManager = webSocketConnectionManager;
    }

    public virtual async Task OnConnected(WebSocket socket)
    {
      await Task.Run(() => WebSocketConnectionManager.AddSocket(socket));
    }

    public virtual async Task OnDisconnected(WebSocket socket)
    {
      await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));
    }

    public async Task SendMessageAsync(WebSocket socket, string message)
    {
      if (socket.State != WebSocketState.Open)
        return;

      var array = Encoding.UTF8.GetBytes(message);

      try
      {
        await socket.SendAsync(buffer: new ArraySegment<byte>(array: array,
            offset: 0,
            count: array.Length),
          messageType: WebSocketMessageType.Text,
          endOfMessage: true,
          cancellationToken: CancellationToken.None);
      }
      catch (WebSocketException e)
      {
        System.Console.WriteLine("Discarding socket: " + e.Message);
        await OnDisconnected(socket);
      }
    }

    public async Task SendMessageAsync(string socketId, string message)
    {
      await SendMessageAsync(WebSocketConnectionManager.GetSocketById(socketId), message);
    }

    public async Task SendMessageToAllAsync(string message)
    {
      foreach (var pair in WebSocketConnectionManager.GetAll())
      {
        if (pair.Value.State == WebSocketState.Open)
          await SendMessageAsync(pair.Value, message);
      }
    }

    public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
  }
}
