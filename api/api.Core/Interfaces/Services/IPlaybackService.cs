using System.Threading.Tasks;
using api.Domain.Entities;

namespace api.Domain.Interfaces.Services
{
    public interface IPlaybackService
    {
        Task StartOrResume(Party party);
    }
}