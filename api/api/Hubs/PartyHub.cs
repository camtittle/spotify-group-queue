using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;


namespace api.Hubs
{
    public class PartyHub : Hub
    {
        private static IHubContext<PartyHub> _hubContext;

        public PartyHub(IHubContext<PartyHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("receiveMessage", user, message);
        }

        public static async Task SendMessageStatic(string user, string message)
        {
            await _hubContext.Clients.All.SendAsync("receiveMessage", user, message);
        }

        

    }
}
