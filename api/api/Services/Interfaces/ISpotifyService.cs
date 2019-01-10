using System.Threading.Tasks;
using api.Controllers.Models;
using api.Models;

namespace api.Services.Interfaces
{
    public interface ISpotifyService
    {
        Task<SpotifyAuthorizationResponse> AuthorizeClient(User user, string code);
    }
}