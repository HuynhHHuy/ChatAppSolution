using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using ChatApp.Server.Services;
using System.Text;
using ChatApp.Common.DAO;
using static System.Net.Mime.MediaTypeNames;

namespace ChatApp.Server.Hubs
{
    public class ChatOneOnOneHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> UserConnections = new();
        private readonly CloudinaryService _cloudinaryService;

        public ChatOneOnOneHub(CloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        public Task Register(string email)
        {
            UserConnections[email] = Context.ConnectionId;
            return Task.CompletedTask;
        }

        public async Task SendMessage(string receiverEmail, byte[] data, string senderEmail, string messageType)
        {
            string payload = string.Empty;

            Console.WriteLine($"Sender: {senderEmail}, Receiver: {receiverEmail}, MessageType: {messageType}");

            if (messageType == "text")
            {
                payload = Encoding.UTF8.GetString(data);
            }
            else if (messageType == "image")
            {
                string fileName = $"img_{Guid.NewGuid()}.jpg";
                string imageUrl = await _cloudinaryService.UploadImageAsync(data, fileName, "chat_images");
                MessageDAO.Instance.InsertMessage(receiverEmail, senderEmail, imageUrl, "image", DateTime.Now);
                payload = imageUrl;
            }

            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            if (UserConnections.TryGetValue(receiverEmail, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId)
                    .SendAsync("ReceiveMessage", senderEmail, payloadBytes, messageType);
            }

            if (UserConnections.TryGetValue(senderEmail, out var senderConnectionId))
            {
                await Clients.Client(senderConnectionId)
                    .SendAsync("ReceiveMessage", senderEmail, payloadBytes, messageType);
            }
        }



        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (user.Key != null)
            {
                UserConnections.TryRemove(user.Key, out _);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
