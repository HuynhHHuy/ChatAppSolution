using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp.Client.Hub
{
    public class ChatOneOnOneHub : IAsyncDisposable
    {
        private HubConnection _connection;
        private bool _isDisposed = false;
        private string _myEmail = string.Empty;

        public ChatOneOnOneHub(string myEmail)
        {
            _myEmail = myEmail;
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/socket/chat-single")
                .WithAutomaticReconnect()
                .Build();
        }

        public async Task ConnectAsync(Action<string, string> onMessageReceived)
        {
            if (_isDisposed) return;

            _connection.On<string, string>("ReceiveMessage", (senderId, message) =>
            {
                onMessageReceived?.Invoke(senderId, message);
            });

            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
                await _connection.InvokeAsync("Register", _myEmail);
            }
        }

        public async Task SendMessageAsync(string receiverEmail, string message)
        {
            if (_isDisposed) return;

            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
                await _connection.InvokeAsync("Register", _myEmail);
            }

            await _connection.InvokeAsync("SendMessage", receiverEmail, message, _myEmail);
        }

        public async ValueTask DisposeAsync()
        {
            if (!_isDisposed)
            {
                await _connection.DisposeAsync();
                _isDisposed = true;
            }
        }
    }
}
