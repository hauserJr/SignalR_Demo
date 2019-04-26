using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SingleR.Hubs
{
    public static class Data
    {
        public static List<Talker> _talkers;
        public static List<Talker> talkers
        {
            get
            {
                if (_talkers == null)
                {
                    _talkers = new List<Talker>();
                }
                return _talkers;
            }
        }
    }

    public class ChatHub : Hub
    {
        
        public async Task SendMessage(string user, string message,string to)
        {
            var queryConnection = Context.ConnectionId;
            //Clients[]
            var query = Data.talkers.Where(o => o.ConnectionId == queryConnection && o.IsNewUser).FirstOrDefault();
            
            if (query != null)
            {
                query.IsNewUser = false;
                query.Name = user;
                query.SecrectCode = user;
            }
            var ToConnection = Data.talkers.Where(o => o.SecrectCode == to).FirstOrDefault();
            if (ToConnection == null)
            {
                await Clients.All.SendAsync("ReceiveMessage", user, message, to);
            }

            await Clients.Client(ToConnection.ConnectionId).SendAsync("ReceiveMessage", user, message, to);
        }
        public override Task OnConnectedAsync()
        {
            var UserComing = Data.talkers.Where(o => o.ConnectionId == Context.ConnectionId).FirstOrDefault();
            if (UserComing == null)
            {
                Data.talkers.Add(new Talker()
                {
                    ConnectionId = Context.ConnectionId
                });
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            return base.OnDisconnectedAsync(ex);
        }
    }

    public class Talker
    {
        public string ConnectionId { get; set; }

        public string Name { get; set; }

        public string SecrectCode { get; set; }

        public bool IsNewUser { get; set; } = true;
    }
}
