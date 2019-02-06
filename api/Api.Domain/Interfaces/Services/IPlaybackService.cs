using System.Threading.Tasks;
using Api.Domain.DTOs;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces.Services
{
    public interface IPlaybackService
    {
        Task StartOrResume(Party party);
        Task UpdatePlaybackState(Party party, SpotifyPlaybackState state, User[] dontNotifyUsers = null);
        Task UpdatePlaybackState(Party party, QueueItem queueItem, bool isPlaying, User[] dontNotifyUsers = null);
        Task SendPlaybackStatusUpdate(Party party, User[] exceptUsers = null);
    }
}