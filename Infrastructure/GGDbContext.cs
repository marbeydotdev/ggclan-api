using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

// ReSharper disable once InconsistentNaming
public class GGDbContext : DbContext
{
    public GGDbContext(DbContextOptions<GGDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Clan> Clans { get; set; }
}