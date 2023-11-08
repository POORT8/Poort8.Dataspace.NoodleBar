using Microsoft.EntityFrameworkCore;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.AuthorizationRegistry;
public class AuthorizationContext : DbContext
{
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<Policy> Policies { get; set; }

    public AuthorizationContext(DbContextOptions<AuthorizationContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>()
            .HasMany(o => o.Employees)
            .WithOne()
            .IsRequired();

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Features)
            .WithMany();

        modelBuilder.Entity<Organization>()
            .HasMany(e => e.Properties)
            .WithOne()
            .IsRequired();

        modelBuilder.Entity<Employee>()
            .HasMany(e => e.Properties)
            .WithOne()
            .IsRequired();

        modelBuilder.Entity<Product>()
            .HasMany(e => e.Properties)
            .WithOne()
            .IsRequired();

        modelBuilder.Entity<Feature>()
            .HasMany(e => e.Properties)
            .WithOne()
            .IsRequired();

        modelBuilder.Entity<Policy>()
            .HasMany(e => e.Properties)
            .WithOne()
            .IsRequired();
    }
}