using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp.Client.Hub
{
    public class StatusAccountHub
    {
        private HubConnection _connection;

        public StatusAccountHub()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/socket/status")
                .WithAutomaticReconnect()
                .Build();
        }
        public async Task ConnectAsync(Action<string, string> onFriendStatusChanged)
        {
            _connection.On<string, string>("FriendStatusChanged", (friendEmail, status) =>
            {
                onFriendStatusChanged?.Invoke(friendEmail, status);
            });

            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
            }
        }

        public async Task SetOnline(string email)
        {
            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
            }

            await _connection.InvokeAsync("SetOnline", email);
        }

        public async Task SetOffline(string email)
        {
            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
            }
            await _connection.InvokeAsync("SetOffline", email);
        }
        public void Disconnect()
        {
            _connection.StopAsync();
            _connection.DisposeAsync();
        }
    }
}
