using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Hubs.Models;
using api.Models;
using PlaybackState = Spotify.Models.PlaybackState;

namespace api.Services.Interfaces
{
    public interface IPartyService
    {
        Task<List<Party>> GetAll();
        Task<Party> Find(string id);
        Task<Party> Create(User owner, string name);
        Task RequestToJoin(Party party, User user);
        Task Delete(Party party);
        Task Leave(User user);
        Task AddPendingMember(Party party, User user);
        Task RemovePendingMember(Party party, User user);
        Task<CurrentParty> GetCurrentParty(Party party, bool partial = false);
        Task<Party> LoadFull(Party party);
        Task<QueueItem> AddQueueItem(User user, AddTrackToQueueRequest request);
        Task RemoveQueueItem(User user, string queueItemId);
        Task UpdateDevice(Party party, string deviceId, string deviceName);
        Task UpdatePlaybackState(Party party, PlaybackState state, User[] dontNotifyUsers = null);
        Task SendPlaybackStatusUpdate(Party party, User[] exceptUsers = null);
        PlaybackStatusUpdate GetPlaybackStatusUpdate(Party party, bool includeAdminFields = false);
    }
}
