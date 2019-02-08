using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Api.Domain.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Spotify.Exceptions;
using Spotify.Interfaces;
using Spotify.Models;

namespace Spotify
{
    public class SpotifyClient : ISpotifyClient
    {
        private readonly ISpotifyTokenManager _spotifyTokenManager;

        private const string SearchEndpoint = "/search";

        private readonly SpotifySettings _settings;

        public SpotifyClient(ISpotifyTokenManager spotifyTokenManager, IOptions<SpotifySettings> settings)
        {
            _spotifyTokenManager = spotifyTokenManager;
            _settings = settings.Value;
        }

        public async Task<TrackSearchResponse> Search(string query)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await _spotifyTokenManager.GetAccessToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                    {
                        { "q", query },
                        { "type", "track" }
                    });
                    var queryString = content.ReadAsStringAsync().Result;
                    var requestUrl = _settings.BaseApiUri + SearchEndpoint + "?" + queryString;

                    var response = await client.GetStringAsync(requestUrl);

                    return JsonConvert.DeserializeObject<TrackSearchResponse>(response);
                }
                catch (Exception e)
                {
                    throw new SpotifyException("Error executing search query: " + query, e);
                }
            }
        }

        /*
         * Requests a new access token from Spotify using an authorization code or refresh token
         */
        public async Task<AuthorizationCodeTokenResponse> GetClientToken(string code, bool isRefreshToken = false)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var credentials = _spotifyTokenManager.GetCredentialsBase64();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                    {
                        {"grant_type", isRefreshToken ? "refresh_token" : "authorization_code"},
                        {isRefreshToken ? "refresh_token" : "code", code},
                        {"redirect_uri", _settings.RedirectUri}
                    });

                    var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenUri)
                    {
                        Content = content
                    };

                    var response = await client.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new SpotifyAuthenticationException(
                            "Spotify authorization code token request / refresh request was unsuccessful. Server responded with: " +
                            response.StatusCode);
                    }

                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseModel = JsonConvert.DeserializeObject<AuthorizationCodeTokenResponse>(responseString);

                    return responseModel;
                }
                catch (Exception e)
                {
                    throw new SpotifyAuthenticationException("Spotify authorization code token request / refresh request was unsuccessful.", e);
                }
            }
        }

        /*
         * Performs a GET request on behalf of an authorised user, using the given access token
         */
        public async Task<T> GetAsUser<T>(string endpoint, string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var uri = _settings.BaseApiUri + endpoint;

                try
                {
                    var response = await client.GetStringAsync(uri);

                    return JsonConvert.DeserializeObject<T>(response);
                }
                catch (HttpRequestException)
                {
                    return default(T);
                }
            }
        }

        public async Task<T> PutAsUser<T>(string endpoint, string accessToken, object jsonBody)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var uri = _settings.BaseApiUri + endpoint;

                try
                {
                    var jsonString = JsonConvert.SerializeObject(jsonBody);
                    var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    var response = await client.PutAsync(uri, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new SpotifyException("Error code return from " + endpoint + ". Status code: " + response.StatusCode);
                    }

                    var responseString = await response.Content.ReadAsStringAsync();
                    
                    return JsonConvert.DeserializeObject<T>(responseString);
                }
                catch (HttpRequestException)
                {
                    return default(T);
                }
            }
        }
    }
}