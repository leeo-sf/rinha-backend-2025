using Microsoft.EntityFrameworkCore;
using RinhaBackend.Api.Config;
using RinhaBackend.Api.Data.Entity;

namespace RinhaBackend.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Payment> Payments { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PaymentEntityTypeConfiguration());
    }
}