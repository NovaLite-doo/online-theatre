using Logic.Entities;

namespace Logic.Repositories;

public class MovieRepository : Repository<Movie>
{
    public MovieRepository(OnlineTheatreDbContext context) : base(context)
    {
    }
}