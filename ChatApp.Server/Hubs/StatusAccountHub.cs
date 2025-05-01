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
            if (!UserConnections.ContainsKey(email))
            {
                UserConnections[email] = Context.ConnectionId;
            }

            UserStatuses[email] = "online";

            var friends = GetUserFriends(email);

            foreach (var friend in friends)
            {
                if (UserConnections.ContainsKey(friend))
                {
                    await Clients.Client(UserConnections[friend]).SendAsync("FriendStatusChanged", email, "online");
                }
            }
        }

        public async Task SetOffline(string email)
        {
            if (!UserConnections.ContainsKey(email))
            {
                UserConnections[email] = Context.ConnectionId;
            }

            UserStatuses[email] = "offline";

            var friends = GetUserFriends(email);

            foreach (var friend in friends)
            {
                if (UserConnections.ContainsKey(friend))
                {
                    await Clients.Client(UserConnections[friend])
                                 .SendAsync("FriendStatusChanged", email, "Offline");
                }
            }
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            var user = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (user.Key != null)
            {
                UserConnections.Remove(user.Key);
                await SetOffline(user.Key);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private List<string> GetUserFriends(string email)
        {
            List<UserDTO> listFriend = new List<UserDTO>();

            listFriend = FriendDAO.Instance.GetFriends(email);

            List<string> friendEmails = listFriend.Select(friend => friend.Email).ToList();

            return friendEmails;
        }

    }
}
