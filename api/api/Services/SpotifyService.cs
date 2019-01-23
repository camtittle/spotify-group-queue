using System;
using System.Collections.Generic;
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

            if (user.SpotifyAccessToken == null || user.SpotifyRefreshToken == null || user.SpotifyTokenExpiry == null)
            {
                throw new SpotifyAuthenticationException("Unable to refresh token without an existing access token");
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

        public async Task UpdateDevice(User user, string deviceId, string deviceName)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentNullException(nameof(deviceId));
            }

            if (string.IsNullOrWhiteSpace(deviceName))
            {
                throw new ArgumentNullException(nameof(deviceName));
            }

            if (user.SpotifyRefreshToken == null)
            {
                throw new SpotifyPlaybackException("Cannot transfer playback - User not authorized with Spotify");
            }

            if (user.CurrentDevice.DeviceId != deviceId)
            {
                await TransferSpotifyPlayback(user, deviceId);
            }

            user.CurrentDevice.DeviceId = deviceId;
            user.CurrentDevice.Name = deviceName;

            await _userService.Update(user);
        }

        private async Task TransferSpotifyPlayback(User user, string deviceId)
        {
            var accessToken = await GetUserAccessToken(user);

            var request = new TransferPlaybackRequest()
            {
                DeviceIds = new[] { deviceId },
                Play = true
            };

            // Transfer Spotify playback to given device
            await _spotifyClient.PutAsUser<string>("/me/player", accessToken.AccessToken, request);
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

    }
}
