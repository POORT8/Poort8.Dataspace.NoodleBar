using Microsoft.EntityFrameworkCore;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.AuthorizationRegistry;
public class AuthorizationContext : DbContext
{
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Entities.Action> Actions { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Policy> Policies { get; set; }

    public AuthorizationContext(DbContextOptions<AuthorizationContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasMany(e => e.Services)
            .WithMany(e => e.Products);
    }
}