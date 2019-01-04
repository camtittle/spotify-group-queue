using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Spotify.Exceptions;
using Spotify.Interfaces;
using Spotify.Models;

namespace Spotify
{
    public class SpotifyClient : ISpotifyClient
    {
        private ISpotifyTokenManager _spotifyTokenManager;

        private const string SearchUrl = "https://api.spotify.com/v1/search";

        public SpotifyClient(ISpotifyTokenManager spotifyTokenManager)
        {
            _spotifyTokenManager = spotifyTokenManager;
        }

        public async Task<SpotifyTrackSearchResponse> Search(string query)
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
                    var requestUrl = SearchUrl + "?" + queryString;

                    var response = await client.GetStringAsync(requestUrl);

                    return JsonConvert.DeserializeObject<SpotifyTrackSearchResponse>(response);
                }
                catch (Exception e)
                {
                    throw new SpotifyException("Error executing search query: " + query, e);
                }
            }
        }
    }
}