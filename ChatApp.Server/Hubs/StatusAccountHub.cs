using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Common.DAO;
using ChatApp.Common.DTOs;

namespace ChatApp.Server.Hubs
{
    public class StatusAccountHub : Hub
    {
        private static readonly Dictionary<string, string> UserConnections = new();
        private static readonly Dictionary<string, string> UserStatuses = new();
        private static readonly Dictionary<string, List<string>> UserFriends = new();

        public async Task SetOnline(string email)
        {
            Console.WriteLine($"[SetOnline] {email} is now online with ConnectionId: {Context.ConnectionId}");

            UserConnections[email] = Context.ConnectionId;
            UserStatuses[email] = "online";

            var friends = GetUserFriends(email);

            foreach (var friend in friends)
            {
                if (UserConnections.TryGetValue(friend, out var friendConnectionId))
                {
                    Console.WriteLine($"[NotifyFriend] Notifying {friend} about {email}'s online status (ConnectionId: {friendConnectionId})");
                    await Clients.Client(friendConnectionId).SendAsync("FriendStatusChanged", email, "online");
                }
                else
                {
                    Console.WriteLine($"[NotifyFriend] {friend} is not connected, skipping.");
                }
            }
        }

        public async Task SetOffline(string email)
        {
            Console.WriteLine($"[SetOffline] {email} is now offline");

            UserStatuses[email] = "offline";

            var friends = GetUserFriends(email);

            foreach (var friend in friends)
            {
                if (UserConnections.TryGetValue(friend, out var friendConnectionId))
                {
                    Console.WriteLine($"[NotifyFriend] Notifying {friend} about {email}'s offline status (ConnectionId: {friendConnectionId})");
                    await Clients.Client(friendConnectionId).SendAsync("FriendStatusChanged", email, "offline");
                }
                else
                {
                    Console.WriteLine($"[NotifyFriend] {friend} is not connected, skipping.");
                }
            }
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            var user = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (user.Key != null)
            {
                Console.WriteLine($"[Disconnected] {user.Key} disconnected (ConnectionId: {Context.ConnectionId})");
                UserConnections.Remove(user.Key);
                await SetOffline(user.Key);
            }
            else
            {
                Console.WriteLine($"[Disconnected] Unknown user disconnected (ConnectionId: {Context.ConnectionId})");
            }

            await base.OnDisconnectedAsync(exception);
        }

        private List<string> GetUserFriends(string email)
        {
            try
            {
                var listFriend = FriendDAO.Instance.GetFriends(email);
                var friendEmails = listFriend.Select(friend => friend.Email).ToList();

                Console.WriteLine($"[GetUserFriends] {email} has {friendEmails.Count} friends: {string.Join(", ", friendEmails)}");

                return friendEmails;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUserFriends] Error for {email}: {ex.Message}");
                return new List<string>();
            }
        }
    }

}
