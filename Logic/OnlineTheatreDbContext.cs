using Logic.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logic;

public class OnlineTheatreDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Movie> Movies { get; set; } = null!;
    public DbSet<PurchasedMovie> PurchasedMovies { get; set; } = null!;

    public OnlineTheatreDbContext(DbContextOptions<OnlineTheatreDbContext> options) : base(options)
    {
    }
}