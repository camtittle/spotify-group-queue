using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Spotify.Exceptions;
using Spotify.Interfaces;
using Spotify.Models;

namespace Spotify.Models
{
    public class SpotifyTokenManager : ISpotifyTokenManager
    {
        private readonly string _credentialsBase64;
        private string _accessToken;
        private DateTime _tokenExpiry;
        
        private const string GrantType = "client_credentials";

        private readonly SpotifySettings _settings;

        public SpotifyTokenManager(IOptions<SpotifySettings> spotifySettings)
        {
            _settings = spotifySettings.Value;
            var credentialsBytes = System.Text.Encoding.UTF8.GetBytes(_settings.ClientId + ":" + _settings.ClientSecret);
            _credentialsBase64 = System.Convert.ToBase64String(credentialsBytes);
        }

        /*
         * Provides a stored Spotify access token, refreshing if necessary
         */
        public async Task<string> GetAccessToken()
        {
            var now = DateTime.UtcNow;
            if (_tokenExpiry < now)
            {
                await RequestServerToken();
            }

            return _accessToken;
        }

        /*
         * Returns the base64 encoded client credentials
         */
        public string GetCredentialsBase64()
        {
            return _credentialsBase64;
        }

        /*
         * Requests an access token from Spotify using the app's client key and secret
         */
        private async Task RequestServerToken()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentialsBase64);

                try
                {
                    var dict = new Dictionary<string, string>
                    {
                        {"grant_type", GrantType}
                    };

                    var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenUri)
                    {
                        Content = new FormUrlEncodedContent(dict)
                    };

                    var response = await client.SendAsync(request);
                    var responseString = await response.Content.ReadAsStringAsync();
                    var accessTokenModel = JsonConvert.DeserializeObject<ClientCredentialsTokenResponse>(responseString);

                    // Verify response contains an access token
                    if (string.IsNullOrWhiteSpace(accessTokenModel.AccessToken))
                    {
                        throw new SpotifyAuthenticationException("Spotify authorization response missing access token");
                    }

                    _accessToken = accessTokenModel.AccessToken;
                    _tokenExpiry = DateTime.Now.AddMilliseconds(accessTokenModel.ExpiresIn);
                }
                catch (Exception e)
                {
                    throw new SpotifyAuthenticationException("Error requesting access token from Spotify", e);
                }
            }
        }

    }
}
