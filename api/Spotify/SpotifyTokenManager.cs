using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Spotify.Interfaces;

namespace Spotify
{
    public class SpotifyTokenManager : ISpotifyTokenManager
    {
        private readonly string _credentialsBase64;
        private string _accessToken;
        private DateTime _tokenExpiry;

        private const string TokenUrl = "https://accounts.spotify.com/api/token";
        private const string GrantType = "client_credentials";

        public SpotifyTokenManager(string clientId, string clientSecret)
        {
            var credentialsBytes = System.Text.Encoding.UTF8.GetBytes(clientId + ":" + clientSecret);
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
                await RequestToken();
            }

            return _accessToken;
        }

        /**
         * Requests an access token from Spotify using the app's client key and secret
         */
        private async Task RequestToken()
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

                    var request = new HttpRequestMessage(HttpMethod.Post, TokenUrl)
                    {
                        Content = new FormUrlEncodedContent(dict)
                    };

                    var response = await client.SendAsync(request);
                    var responseString = await response.Content.ReadAsStringAsync();
                    var accessTokenModel = JsonConvert.DeserializeObject<AuthorizationResponseModel>(responseString);

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
