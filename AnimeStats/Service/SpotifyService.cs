using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AnimeStats.Service
{
    public class SpotifyService : ISpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId = "YOUR_CLIENT_ID";
        private readonly string _clientSecret = "YOUR_CLIENT_SECRET";

        public SpotifyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            request.Content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "client_credentials")


        });

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(responseContent);
            return data.GetProperty("access_token").GetString();
        }

        public async Task<List<Song>> GetTopSongsAsync()
        {
            var token = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("https://api.spotify.com/v1/search?q=year:2000-2009&type=track&limit=100&market=US");
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(responseContent);

            var songs = new List<Song>();
            var tracks = data.GetProperty("tracks").GetProperty("items");

            for (int i = 0; i < tracks.GetArrayLength(); i++)
            {
                var track = tracks[i];
                songs.Add(new Song
                {
                    Index = i + 1,
                    Name = track.GetProperty("name").GetString(),
                    ReleaseDate = track.GetProperty("album").GetProperty("release_date").GetString(),
                    Popularity = track.GetProperty("popularity").GetInt32()
                });
            }

            return songs;
        }
    }

    public class Song
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string ReleaseDate { get; set; }
        public int Popularity { get; set; }
    }

    public interface ISpotifyService
    {
        Task<List<Song>> GetTopSongsAsync();
    }
}
