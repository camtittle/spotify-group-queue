using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.DTOs;
using Api.Domain.Entities;
using PlaybackState = Spotify.Models.PlaybackState;

namespace Api.Domain.Interfaces.Services
{
    public interface IPartyService
    {
        Task<Party> Create(User owner, string name);
        Task Delete(Party party);
        Task<QueueItem> AddQueueItem(User user, AddTrackToQueueRequest request);
        Task RemoveQueueItem(User user, string queueItemId);
        Task<SpotifyDevice> UpdateDevice(Party party, string deviceId, string deviceName);
        Task UpdatePlaybackState(Party party, PlaybackState state, User[] dontNotifyUsers = null);
        Task UpdatePlaybackState(Party party, QueueItem queueItem, bool isPlaying, User[] dontNotifyUsers = null);
        Task SendPlaybackStatusUpdate(Party party, User[] exceptUsers = null);
        PlaybackStatusUpdate GetPlaybackStatusUpdate(Party party, bool includeAdminFields = false);
        Task<QueueItem> RemoveNextQueueItem(Party party);
    }
}
