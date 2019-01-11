using System;
using System.Threading.Tasks;
using api.Controllers.Models;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Spotify.Exceptions;
using Spotify.Interfaces;
using Spotify.Models;

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
        public async Task<SpotifyAuthorizationResponse> AuthorizeClient(User user, string code)
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

                var responseModel = new SpotifyAuthorizationResponse(result);

                return responseModel;
            }
            catch (Exception e)
            {
                throw new SpotifyAuthenticationException("Unable to exchange code for token with Spotify.", e);
            }
        }

        /*
         * Returns a new Spotify access token for Spotify account associated with the given user
         */
        public async Task<SpotifyAuthorizationResponse> RefreshClientToken(User user)
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
                return new SpotifyAuthorizationResponse(user.SpotifyAccessToken, expiresIn);
            }

            // Get a new token
            var result = await _spotifyClient.GetClientToken(user.SpotifyRefreshToken, true);

            await UpdateUserTokens(user, result.AccessToken, result.RefreshToken, result.ExpiresIn);

            return new SpotifyAuthorizationResponse(result);
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

            user.SpotifyDeviceId = deviceId;
            user.SpotifyDeviceName = deviceName;

            await _userService.Update(user);
        }


    }
}
