using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using api.Controllers.Models;
using api.Exceptions;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Spotify.Exceptions;
using Spotify.Interfaces;
using Spotify.Models;
using PlaybackState = Spotify.Models.PlaybackState;

namespace api.Services
{
    public class SpotifyService : ISpotifyService
    {

        private readonly ISpotifyClient _spotifyClient;
        private readonly IUserService _userService;

        public SpotifyService(ISpotifyClient spotifyClient,
                              IUserService userService)
        {
            _spotifyClient = spotifyClient;
            _userService = userService;
        }

        /*
         * Exchange an authorization code for an Access token and Refresh token from Spotify.
         * Stores the credentials against the user
         */
        public async Task<SpotifyAccessToken> AuthorizeClient(User user, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                // Attempt to exchange code for access token 
                var result = await _spotifyClient.GetClientToken(code);
                
                await UpdateUserTokens(user, result.AccessToken, result.RefreshToken, result.ExpiresIn);

                return new SpotifyAccessToken(result);
            }
            catch (Exception e)
            {
                throw new SpotifyAuthenticationException("Unable to exchange code for token with Spotify.", e);
            }
        }

        /*
         * Returns a Spotify access token for Spotify account associated with the given user
         * If there is a valid one stored, use that, else fetch a new one from Spotify
         */
        public async Task<SpotifyAccessToken> GetUserAccessToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.SpotifyRefreshToken == null || user.SpotifyTokenExpiry == null)
            {
                throw new SpotifyAuthenticationException("Unable to refresh token without an existing refresh token and expiry");
            }
            
            // Check if existing token is still valid
            var now = DateTime.UtcNow;
            var expiresIn = Convert.ToInt32(((DateTime)user.SpotifyTokenExpiry - now).TotalSeconds);

            // If there's more than 3 minutes left, use it. Else refresh it
            if (expiresIn > 180)
            {
                return new SpotifyAccessToken(user.SpotifyAccessToken, expiresIn);
            }

            // Get a new token
            var result = await _spotifyClient.GetClientToken(user.SpotifyRefreshToken, true);

            await UpdateUserTokens(user, result.AccessToken, result.RefreshToken, result.ExpiresIn);

            return new SpotifyAccessToken(result);
        }

        public async Task UpdateUserTokens(User user, string accessToken, string refreshToken, int expiresIn)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            var expiry = DateTime.UtcNow.AddSeconds(expiresIn);

            user.SpotifyAccessToken = accessToken;
            user.SpotifyTokenExpiry = expiry;
            if (refreshToken != null)
            {
                user.SpotifyRefreshToken = refreshToken;
            }

            await _userService.Update(user);
        }

        public async Task<bool> TransferPlayback(User user, string deviceId)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentNullException(nameof(deviceId));
            }

            if (user.SpotifyRefreshToken == null)
            {
                throw new SpotifyPlaybackException("Cannot transfer playback - User not authorized with Spotify");
            }

            if (user.CurrentDevice.DeviceId == deviceId)
            {
                return false;
            }
            
            var accessToken = await GetUserAccessToken(user);

            var request = new TransferPlaybackRequest()
            {
                DeviceIds = new[] { deviceId },
                Play = true
            };

            //Attempt transfer Spotify playback to given device
            try
            {
                await _spotifyClient.PutAsUser<string>("/me/player", accessToken.AccessToken, request);
                return true;
            }
            catch (SpotifyException e)
            {
                return false;
            }
            
        }

        public async Task<PlaybackState> GetPlaybackState(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.SpotifyRefreshToken == null)
            {
                throw new SpotifyAuthenticationException("Error getting playback state: User not authorized with Spotify");
            }

            var accessToken = await GetUserAccessToken(user);

            return await _spotifyClient.GetAsUser<PlaybackState>("/me/player", accessToken.AccessToken);
        }

        public async Task PlayTrack(User user, string uri, int startAtMillis = 0)
        {
            await PlayTrack(user, new[] { uri }, startAtMillis);
        }

        public async Task PlayTrack(User user, string[] uris, int startAtMillis = 0)
        {
            if (uris == null || uris.Length < 1)
            {
                throw new ArgumentNullException(nameof(uris));
            }

            if (startAtMillis < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startAtMillis));
            }

            var accessToken = await GetUserAccessToken(user);

            var body = new
            {
                uris,
                position_ms = startAtMillis
            };

            await _spotifyClient.PutAsUser<string>("/me/player/play", accessToken.AccessToken, body);
        }

    }
}
