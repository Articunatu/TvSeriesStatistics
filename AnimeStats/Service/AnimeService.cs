using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

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
        public int MinutesPerEpisode { get; set; }
        public int AmtOfEpisodes { get; set; }
        public DateTime Premier { get; set; }
        public DateTime Finale { get; set; }
    }

    class AnimeResponse
    {
        public List<AnimeData> Data { get; set; }
    }

    public class AnimeData
    {
        public string Title_English { get; set; }
        public double Score { get; set; }
        public int ScoredBy { get; set; }
        public DateTime AiredFrom { get; set; }
        public DateTime AiredTo { get; set; }
    }

    public class AnimeService(DatabaseEFCore database) : IAnimeService
    {
        readonly DatabaseEFCore db = database;
        readonly HttpClient httpClient = new();

        public async Task<IEnumerable<Anime>> SetPopularAnime()
        {
            List<Anime> animeList = [];
            var response = await httpClient.GetAsync("https://api.jikan.moe/v4/top/anime?type=tv&filter=bypopularity&limit=10");
            response.EnsureSuccessStatusCode(); // Ensure a successful response

            var animeResponse = await response.Content.ReadFromJsonAsync<AnimeResponse>();
            foreach (var data in animeResponse.Data)
            {
                var title = data.Title_English;
                Anime? existingAnime = db.Animes.FirstOrDefault(a => a.Title.StartsWith(title.Substring(0, 8)));
                if (existingAnime != null)
                {
                    Season season = new()
                    {
                        AnimeId = existingAnime.Id,
                        Number = existingAnime.Seasons.Count + 1,
                        AudienceScore = data.Score,
                        Premier = data.AiredFrom,
                        Finale = data.AiredTo,
                        AmtOfScorers = data.ScoredBy,
                    };
                    existingAnime.Seasons.Add(season);
                    db.Animes.Update(existingAnime);
                    await db.SaveChangesAsync();
                }
                else
                {
                    var anime = new Anime
                    {
                        Title = data.Title_English,

                    };
                    animeList.Add(anime);
                }
            }
            return animeList;
        }
    }

    public interface IAnimeService
    {
        Task<IEnumerable<Anime>> SetPopularAnime(); 
    }
}