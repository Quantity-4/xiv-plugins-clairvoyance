using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.WebSockets;


namespace Clairvoyance.server
{
    public class RealTimeModule : WebSocketModule
    {
        public RealTimeModule(string urlPath) : base(urlPath, true)
        {
        }

        protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer,
            IWebSocketReceiveResult result)
        {
            // Handle incoming messages if needed
            return Task.CompletedTask;
        }

        protected override Task OnClientConnectedAsync(IWebSocketContext context)
        {
            // Notify that a client has connected, if needed
            return base.OnClientConnectedAsync(context);
        }

        protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
        {
            // Handle client disconnection if needed
            return base.OnClientDisconnectedAsync(context);
        }

        public Task SendToAllAsync(string message)
        {
            return BroadcastAsync(message);
        }
    }
}