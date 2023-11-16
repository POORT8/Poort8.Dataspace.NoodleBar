using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using System.Security.Claims;
using System.Text.Json;

namespace Poort8.Dataspace.AuthorizationRegistry;
public class AuthorizationContext : DbContext
{
    private readonly ClaimsPrincipal? _currentUser;
    public DbSet<AuditRecord> AuditRecords { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<Policy> Policies { get; set; }

    public AuthorizationContext(DbContextOptions<AuthorizationContext> options, IHttpContextAccessor? httpContextAccessor = null) : base(options)
    {
        _currentUser = httpContextAccessor?.HttpContext?.User;
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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var auditRecords = HandleAudit();

        var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await SaveAuditRecordChanges(auditRecords);

        return result;
    }

    private List<AuditRecord> HandleAudit()
    {
        ChangeTracker.DetectChanges();

        var auditRecords = new List<AuditRecord>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditRecord || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditRecord = new AuditRecord(
                DateTime.Now,
                _currentUser?.Identity?.Name ?? "unknown",
                entry.Metadata.ClrType.Name,
                entry.Properties!.First(p => p.Metadata!.IsPrimaryKey()).CurrentValue!.ToString()!,
                entry.State.ToString(),
                JsonSerializer.Serialize(entry.Entity));

            auditRecords.Add(auditRecord);
        }

        return auditRecords;
    }

    private async Task SaveAuditRecordChanges(List<AuditRecord> auditRecords)
    {
        if (auditRecords.Count != 0)
        {
            AuditRecords.AddRange(auditRecords);
            await SaveChangesAsync();
        }
    }
}