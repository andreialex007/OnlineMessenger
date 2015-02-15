using System.Linq;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Infrastructure;

namespace OnlineMessenger.MvcServer.Hubs
{
    public class Broadcaster : IBroadcaster
    {
        protected IHubContext Hub
        {
            get { return GlobalHost.ConnectionManager.GetHubContext<ChatHub>(); }
        }

        #region implementation of IBroadcaster

        public void Broadcast(Message message, string[] userNames)
        {
            foreach (var to in userNames)
                Hub.Clients.User(to).broadcast(Serialize(message));
        }

        public void UserConnected(User user, string[] userNames)
        {
            foreach (var u in userNames)
                Hub.Clients.User(u).userConnected(ConvertUserToUserConnected(user));
        }

        public void UserDisconnected(User user, string[] userNames)
        {
            foreach (var u in userNames)
                Hub.Clients.User(u).userDisconnected(ConvertUserToUserConnected(user));
        }

        #endregion IBroadcaster

        #region private methods

        private static object ConvertUserToUserConnected(User user)
        {
            return Serialize(new { user.Name, user.Id, Roles = user.Roles.Select(x => x.Name) });
        }

        private static object Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }

        #endregion private methods
    }
}