using System.Threading.Tasks;
using api.Controllers.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Spotify.Interfaces;
using Spotify.Models;
using Spotify.Exceptions;

namespace api.Controllers
{
    [Route("api/v1/[controller]")]
    public class SpotifyController : Controller
    {

        private readonly ISpotifyService _spotifyService;
        private readonly IUserService _userService;

        public SpotifyController(ISpotifyService spotifyService, IUserService userService)
        {
            _spotifyService = spotifyService;
            _userService = userService;
        }

        [HttpPost("authorize")]
        [Authorize]
        public async Task<IActionResult> Authorize([FromBody] SpotifyAuthorizationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.GetFromClaims(User);

            // Attempt to exchange the code for an access token
            var result = await _spotifyService.AuthorizeClient(user, request.Code);

            return Ok(result);
        }

        [HttpGet("refresh")]
        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            var user = await _userService.GetFromClaims(User);

            var result = await _spotifyService.RefreshClientToken(user);

            return Ok(result);
        }

        [HttpPost("device")]
        [Authorize]
        public async Task<IActionResult> SetPlaybackDevice([FromBody] SetSpotifyDeviceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.GetFromClaims(User);

            await _spotifyService.UpdateDevice(user, request.DeviceId, request.DeviceName);

            return Ok();
        }
    }
}