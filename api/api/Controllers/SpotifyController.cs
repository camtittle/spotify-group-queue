using System.Threading.Tasks;
using api.Controllers.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/v1/[controller]")]
    public class SpotifyController : Controller
    {

        private readonly ISpotifyService _spotifyService;
        private readonly IUserService _userService;
        private readonly IPartyService _partyService;

        public SpotifyController(ISpotifyService spotifyService, IUserService userService, IPartyService partyService)
        {
            _spotifyService = spotifyService;
            _userService = userService;
            _partyService = partyService;
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
            var token = await _spotifyService.AuthorizeClient(user, request.Code);

            if (user.IsOwner)
            {
                var playbackState = await _spotifyService.GetPlaybackState(user);

                if (playbackState != null)
                {
                    await _partyService.UpdatePlaybackState(user.OwnedParty, playbackState);
                }
            }

            var result = new SpotifyAuthorizationResponse(token.AccessToken, token.ExpiresIn);

            return Ok(result);
        }

        [HttpGet("refresh")]
        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            var user = await _userService.GetFromClaims(User);

            if (user.SpotifyRefreshToken == null)
            {
                return NoContent();
            }

            var result = await _spotifyService.GetUserAccessToken(user);

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