using Api.Domain.DTOs;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces.Helpers
{
    public interface IStatusUpdateHelper
    {
        void CreatePlaybackStatusUpdate(Party party, out PlaybackStatusUpdate memberUpdate, out PlaybackStatusUpdate adminUpdate);
        void CreatePartyStatusUpdate(Party party, out PartyStatus fullMemberUpdate, out PartyStatus pendingMemberUpdate);
    }
}