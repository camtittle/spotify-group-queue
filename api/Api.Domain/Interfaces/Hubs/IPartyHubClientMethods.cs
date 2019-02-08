using System.Threading.Tasks;
using Api.Domain.DTOs;

namespace Api.Domain.Interfaces.Hubs
{
    public interface IPartyHubClientMethods
    {
        Task onPendingMemberRequest(UserPartial pendingUser);
        Task playbackStatusUpdate(PlaybackStatusUpdate statusUpdate);
        Task partyStatusUpdate(PartyStatus statusUpdate);
        Task pendingMembershipResponse(bool accepted);
    }
}