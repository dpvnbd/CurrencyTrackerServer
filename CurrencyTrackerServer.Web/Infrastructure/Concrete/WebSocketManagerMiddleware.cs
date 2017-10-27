using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using CurrencyTrackerServer.Web.Infrastructure.Abstract;
using Microsoft.AspNetCore.Http;

namespace CurrencyTrackerServer.Web.Infrastructure.Concrete
{
  public class WebSocketManagerMiddleware
  {
    private readonly RequestDelegate _next;
    private WebSocketHandler _webSocketHandler { get; set; }

    public WebSocketManagerMiddleware(RequestDelegate next,
      WebSocketHandler webSocketHandler)
    {
      _next = next;
      _webSocketHandler = webSocketHandler;
    }

    public async Task Invoke(HttpContext context)
    {
      if (!context.WebSockets.IsWebSocketRequest)
        return;

      var socket = await context.WebSockets.AcceptWebSocketAsync();
      var connectionId = await _webSocketHandler.OnConnected(socket);

      await Receive(socket, async (result, buffer) =>
      {
        if (result.MessageType == WebSocketMessageType.Text)
        {
          await _webSocketHandler.ReceiveAsync(socket, result, buffer);
          return;
        }

        else if (result.MessageType == WebSocketMessageType.Close)
        {
          await _webSocketHandler.OnDisconnected(socket);
          return;
        }
      });

      //TODO - investigate the Kestrel exception thrown when this is the last middleware
      //await _next.Invoke(context);
    }

    private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
      var buffer = new byte[1024 * 4];
      WebSocketReceiveResult result = null;

      while (socket.State == WebSocketState.Open)
      {
        try
        {
          result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
            cancellationToken: CancellationToken.None);
        }
        catch (WebSocketException e)
        {
          Console.WriteLine(e.WebSocketErrorCode + ": " + e.Message);
          return;
        }
        if (result != null) handleMessage(result, buffer);
      }
    }
  }
}
