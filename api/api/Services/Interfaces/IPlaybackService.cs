using System.Threading.Tasks;
using api.Models;

namespace api.Services.Interfaces
{
    public interface IPlaybackService
    {
        Task StartOrResume(Party party);
    }
}