using CurrencyTrackerServer.Web.Infrastructure.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyTrackerServer.Web.Infrastructure.Websockets
{
    public static class WebsocketMap
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app,
            PathString path,
            WebSocketHandler handler)
        {
            return app.Map(path, (_app) => _app.UseMiddleware<WebSocketManagerMiddleware>(handler));
        }

        
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddSingleton<WebSocketConnectionManager>();

            //foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            //{
            //    if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
            //    {

            //        services.AddSingleton(type);

            //    }
            //}

            return services;
        }
    }
}
