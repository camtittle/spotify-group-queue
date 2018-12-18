using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Hubs.Models;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;


namespace api.Hubs
{
    public class PartyHub : Hub
    {
        private static IHubContext<PartyHub> _hubContext;
        private IUserService _userService;

        private static readonly string ADMIN_GROUP_SUFFIX = "ADMIN";

        public PartyHub(IHubContext<PartyHub> hubContext, IUserService userService)
        {
            _hubContext = hubContext;
            _userService = userService;
        }

        public override async Task OnConnectedAsync()
        {
            var user = await GetCurrentUser();
            var party = _userService.GetParty(user);

            // Add user to a Group with the party ID as its name
            await Groups.AddToGroupAsync(Context.ConnectionId, party.Id);

            // If the user is the owner of the party, add to a group with name {partyID}ADMIN
            var groupName = party.Id + ADMIN_GROUP_SUFFIX;
            if (user.IsOwner)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }

            // TODO: notify clients of new member
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await GetCurrentUser();
            var party = _userService.GetParty(user);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, party.Id);
            if (user.IsOwner)
            {
                var groupName = party.Id + ADMIN_GROUP_SUFFIX;
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("receiveMessage", user, message);
        }

        public static async Task SendMessageStatic(string user, string message)
        {
            await _hubContext.Clients.All.SendAsync("receiveMessage", user, message);
        }

        public static async Task NotifyAdminNewPendingMember(User user, Party party)
        {
            var userModel = new OtherUserModel
            {
                Id = user.Id,
                Username = user.Username
            };
            var adminGroupName = party.Id + ADMIN_GROUP_SUFFIX;
            await _hubContext.Clients.Group(adminGroupName).SendCoreAsync("onPendingMemberRequest", new [] {(object)userModel});
        }

        public async Task<User> GetCurrentUser()
        {
            var userId = Context.User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
            return await _userService.Find(userId);
        }

        

    }
}
