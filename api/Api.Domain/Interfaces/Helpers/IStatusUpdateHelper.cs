using Api.Domain.DTOs;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces.Helpers
{
    public interface IStatusUpdateHelper
    {
        PlaybackStatusUpdate GeneratePlaybackStatusUpdate(Party party, bool includeAdminFields = false);
    }
}