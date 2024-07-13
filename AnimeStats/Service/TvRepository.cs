namespace AnimeStats.Service
{
    public class TvRepository(DatabaseEFCore database) :
        Repository<Anime>(database), IRepository<Anime> { } 
}
