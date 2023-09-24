using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;

public class MarketDbContext : DbContext
{
    public MarketDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Item> Items { get; set; }
}