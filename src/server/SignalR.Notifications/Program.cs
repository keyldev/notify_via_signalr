using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace SignalR.Notifications
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR();

            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");
            app.MapGet("send", async (string msg, IHubContext<NotifyHub> hubContext) =>
            {
                await hubContext.Clients.All.SendAsync("Recieve", $"{msg}");
                Debug.WriteLine(msg);
                return "Sended";
            });

            app.MapHub<NotifyHub>("/notify");

            app.Run();
        }
    }
}