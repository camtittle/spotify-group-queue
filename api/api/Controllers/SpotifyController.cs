using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Interfaces.Helpers;
using Api.Domain.Interfaces.Repositories;
using Api.Domain.Interfaces.Services;
using Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/v1/[controller]")]
    public class SpotifyController : Controller
    {

        private readonly IUserRepository _userRepository;

        private readonly ISpotifyService _spotifyService;
        private readonly IPartyService _partyService;
        private readonly IPlaybackService _playbackService;

        private readonly IJwtHelper _jwtHelper;

        public SpotifyController(IUserRepository userRepository, ISpotifyService spotifyService, IPartyService partyService, IPlaybackService playbackService, IJwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _spotifyService = spotifyService;
            _partyService = partyService;
            _playbackService = playbackService;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("authorize")]
        [Authorize]
        public async Task<IActionResult> Authorize([FromBody] SpotifyAuthorizationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var id = _jwtHelper.GetUserIdFromToken(User);
            var user = await _userRepository.GetById(id);

            // Attempt to exchange the code for an access token
            var token = await _spotifyService.AuthorizeClient(user, request.Code);

            if (user.IsOwner)
            {
                var playbackState = await _spotifyService.GetPlaybackState(user);

                if (playbackState != null)
                {
                    // ToDo: This shouldn't be done in the controller, move to a service
                    await _playbackService.UpdatePlaybackState(user.OwnedParty, playbackState);
                }
            }

            var result = new SpotifyAuthorizationResponse(token.AccessToken, token.ExpiresIn);

            return Ok(result);
        }

        [HttpGet("refresh")]
        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            var id = _jwtHelper.GetUserIdFromToken(User);
            var user = await _userRepository.GetById(id);

            if (user.SpotifyRefreshToken == null)
            {
                return NoContent();
            }

            var result = await _spotifyService.GetUserAccessToken(user);

            return Ok(result);
        }

        // TODO: lock down to party owners
        // ToDo: why is this an endpoint and not a hub method?
        [HttpPost("device")]
        [Authorize]
        public async Task<IActionResult> SetPlaybackDevice([FromBody] SetSpotifyDeviceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = _jwtHelper.GetUserIdFromToken(User);
            var user = await _userRepository.GetById(id);

            if (!user.IsOwner)
            {
                return Forbid("Cannot set playback device - User not owner of any party");
            }

            var party = user.GetActiveParty();

            if (user.CurrentDevice.DeviceId != request.DeviceId)
            {
                var device = await _partyService.UpdateDevice(party, request.DeviceId, request.DeviceName);

                await _playbackService.SendPlaybackStatusUpdate(party);

                return Ok(device);
            }

            return Ok(user.CurrentDevice);
        }

        [HttpPost("testTimer")]
        [Authorize]
        public async Task<IActionResult> TestTimerPlayback()
        {
            var playForMillis = 20000;
            var startAfterMillis = playForMillis;

            // the sound - 1975
            var uri1 = "spotify:track:7h6lpVuSGPW6RNjDXKpYDh";
            var duration1 = 248879;

            // don't go - smokin jack hill
            var uri2 = "spotify:track:4InZWL62blQGzduWwQusYX";
            var duration2 = 240000;

            // true - digital farm animals
            var uri3 = "spotify:track:4OeW3FtleS18prcxVpNAVX";
            var duration3 = 193946;

            // be the one - dua lipa
            var uri4 = "spotify:track:1ixphys4A3NEXp6MDScfih";

            var id = _jwtHelper.GetUserIdFromToken(User);
            var user = await _userRepository.GetById(id);

            if (user.SpotifyRefreshToken == null)
            {
                return BadRequest("Not connected to spotify");
            }

            // Trigger playback of a Spotify track
            await _spotifyService.PlayTrack(user, new []{uri3, uri4}, duration3 - playForMillis);

            // Set up a timer to play another song when that one finishes
            var timer = new Timer(state => { _spotifyService.PlayTrack(user, new []{uri2}); }, null, playForMillis, Timeout.Infinite);

            return Ok();

        }
    }
}