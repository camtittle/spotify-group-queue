using System.Threading.Tasks;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces.Services
{
    public interface IPlaybackService
    {
        Task StartOrResume(Party party);
    }
}