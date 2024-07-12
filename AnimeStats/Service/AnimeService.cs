using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace AnimeStats.Service
{
    public class Anime
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Season> Seasons { get; set; }
    }

    public class Season
    {
        [Key]
        public long Id { get; set; }
        public int Number { get; set; }
        public int AnimeId { get; set; }
        public double AudienceScore { get; set; }
        //public double CriticScore { get; set; }
        //public double AverageScore = Math.Round(((AudienceScore+CriticScore)/2), 2);
        public int AmtOfScorers { get; set; }
        //public int MinutesPerEpisode { get; set; }
        public int AmtOfEpisodes { get; set; }
        public DateTime Premier { get; set; }
        public DateTime Finale { get; set; }
    }

    public record AnimeResponse
    {
        public List<AnimeData> Data { get; set; }
    }

    public record AnimeData
    {
        public string Title_English { get; set; }
        public double Score { get; set; }
        public int ScoredBy { get; set; }
        public Aired? Aired { get; set; }
        public Images? Images { get; set; }
    }

    public record Aired
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public record Images
    {
        public Jpg Jpg { get; set; }
    }

    public record Jpg
    {
        public string Image_Url { get; set; }
    }

    public class AnimeService(DatabaseEFCore database) : IAnimeService
    {
        readonly DatabaseEFCore db = database;
        readonly HttpClient httpClient = new();

        public async Task<IEnumerable<Anime>> SetPopularAnime()
        {
            List<Anime> animeList = new List<Anime>();

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://api.jikan.moe/v4/top/anime?type=tv&filter=bypopularity&limit=10&page=2");
            response.EnsureSuccessStatusCode(); // Ensure a successful response

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new CustomDateTimeConverter() }
            };

            var animeResponse = await response.Content.ReadFromJsonAsync<AnimeResponse>(options);
            if (animeResponse?.Data != null)
            {
                foreach (var data in animeResponse.Data)
                {
                    var title = data?.Title_English ?? string.Empty;
                    string titleSubstring = title.Length > 14 ? title.Substring(0, 8) : title;
                    Anime? existingAnime = db.Animes.FirstOrDefault(a => a.Title.StartsWith(titleSubstring));

                    if (existingAnime != null)
                    {
                        // Ensure the Seasons list is initialized
                        if (existingAnime.Seasons == null)
                        {
                            existingAnime.Seasons = new List<Season>();
                        }

                        Season season = new Season
                        {
                            AnimeId = existingAnime.Id,
                            Number = existingAnime.Seasons.Count + 1,
                            AudienceScore = data?.Score ?? 0,
                            Premier = data.Aired.From,
                            Finale = data.Aired.To,
                            AmtOfScorers = data?.ScoredBy ?? 0,
                        };
                        existingAnime.Seasons.Add(season);
                        db.Animes.Update(existingAnime);
                    }
                    else
                    {
                        Anime anime = new Anime
                        {
                            Title = data?.Title_English ?? string.Empty,
                            Seasons = new List<Season>()
                        };

                        db.Animes.Add(anime);
                        await db.SaveChangesAsync(); // Save the changes to get the anime ID

                        anime.Seasons.Add(new Season
                        {
                            AnimeId = anime.Id,
                            Number = 1,
                            AudienceScore = data?.Score ?? 0,
                            Premier = data.Aired.From,
                            Finale = data.Aired.To,
                            AmtOfScorers = data?.ScoredBy ?? 0,
                        });

                        animeList.Add(anime);
                    }
                }

                await db.SaveChangesAsync(); // Save changes to the database
            }

            return animeList;
        }
    }

    public interface IAnimeService
    {
        Task<IEnumerable<Anime>> SetPopularAnime(); 
    }

    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return DateTime.MinValue; // Return a default value for null DateTime
            }

            if (reader.TryGetDateTime(out DateTime date))
            {
                return date; // Return the parsed DateTime
            }

            // Handle custom date formats or other cases here
            // For example:
            if (DateTime.TryParse(reader.GetString(), out DateTime parsedDate))
            {
                return parsedDate;
            }

            throw new JsonException($"Cannot convert '{reader.GetString()}' to DateTime.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd")); // Customize how DateTime is serialized if needed
        }
    }
}