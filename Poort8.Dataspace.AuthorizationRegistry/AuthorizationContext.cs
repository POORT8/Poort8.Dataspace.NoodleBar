using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Poort8.Dataspace.AuthorizationRegistry.Audit;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using System.Security.Claims;
using System.Text.Json;

namespace Poort8.Dataspace.AuthorizationRegistry;
public class AuthorizationContext : DbContext
{
    private readonly ClaimsPrincipal? _currentUser;
    public DbSet<EntityAuditRecord> EntityAuditRecords { get; set; }
    public DbSet<EnforceAuditRecord> EnforceAuditRecords { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<ResourceGroup> ResourceGroups { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Policy> Policies { get; set; }

    public AuthorizationContext(DbContextOptions<AuthorizationContext> options, IHttpContextAccessor? httpContextAccessor = null) : base(options)
    {
        _currentUser = httpContextAccessor?.HttpContext?.User;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>()
            .ToTable("ArOrganization")
            .HasMany(o => o.Employees)
            .WithOne()
            .IsRequired();

        modelBuilder.Entity<ResourceGroup>()
            .ToTable("ArResourceGroup")
            .HasMany(p => p.Resources)
            .WithMany();

        modelBuilder.Entity<Organization>()
            .ToTable("ArOrganization")
            .HasMany(e => e.Properties)
            .WithOne()
            .IsRequired();

        modelBuilder.Entity<Employee>()
            .ToTable("ArEmployee")
            .HasMany(e => e.Properties)
            .WithOne()
            .IsRequired();

        modelBuilder.Entity<ResourceGroup>()
            .ToTable("ArResourceGroup")
            .HasMany(e => e.Properties)
            .WithOne()
            .IsRequired();

        modelBuilder.Entity<Resource>()
            .ToTable("ArResource")
            .HasMany(e => e.Properties)
            .WithOne()
            .IsRequired();

        modelBuilder.Entity<Policy>()
            .ToTable("ArPolicy")
            .HasMany(e => e.Properties)
            .WithOne()
            .IsRequired();

        modelBuilder.Entity<EnforceAuditRecord>()
            .HasIndex(p => p.Timestamp)
            .IsDescending();

        modelBuilder.Entity<EntityAuditRecord>()
            .HasIndex(p => p.Timestamp)
            .IsDescending();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var auditRecords = HandleAudit();

        var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await SaveAuditRecordChanges(auditRecords);

        return result;
    }

    private List<EntityAuditRecord> HandleAudit()
    {
        ChangeTracker.DetectChanges();

        var auditRecords = new List<EntityAuditRecord>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is EntityAuditRecord ||
                entry.Entity is EnforceAuditRecord ||
                entry.State == EntityState.Detached ||
                entry.State == EntityState.Unchanged)
                continue;

            var auditRecord = new EntityAuditRecord(
                _currentUser?.Identity?.Name ?? "unknown",
                entry.Metadata.ClrType.Name,
                entry.Properties!.First(p => p.Metadata!.IsPrimaryKey()).CurrentValue!.ToString()!,
                entry.State.ToString(),
                JsonSerializer.Serialize(entry.Entity));

            auditRecords.Add(auditRecord);
        }

        return auditRecords;
    }

    private async Task SaveAuditRecordChanges(List<EntityAuditRecord> auditRecords)
    {
        if (auditRecords.Count != 0)
        {
            EntityAuditRecords.AddRange(auditRecords);
            await SaveChangesAsync();
        }
    }
}