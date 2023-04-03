using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace Data.DbContext;

public class HashDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Hash> Hashes { get; set; } = null!; // This is never null

    public HashDbContext(DbContextOptions<HashDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hash>(b =>
        {
            b.HasKey(h => h.Id);
            b.Property(h => h.Id).IsRequired().ValueGeneratedOnAdd();
            b.Property(h => h.Date).IsRequired();
            b.Property(h => h.Sha1).IsRequired();
        });
    }
}
