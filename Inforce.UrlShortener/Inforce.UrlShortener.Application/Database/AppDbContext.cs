using Inforce.UrlShortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inforce.UrlShortener.Application.Database;

public class AppDbContext : DbContext
{
    public DbSet<ShortUrl> ShortUrls { get; set; }
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}