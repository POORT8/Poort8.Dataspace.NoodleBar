using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Poort8.Dataspace.OrganizationRegistry;
public class OrganizationContext : DbContext
{
    public DbSet<AuditRecord> AuditRecords { get; set; }
    public DbSet<Organization> Organizations { get; set; }

    public OrganizationContext(DbContextOptions<OrganizationContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>()
            .OwnsOne(o => o.Adherence);

        modelBuilder.Entity<Organization>()
            .HasMany(o => o.Roles)
            .WithOne()
            .IsRequired();

        modelBuilder.Entity<Organization>()
            .HasMany(o => o.Properties)
            .WithOne()
            .IsRequired();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        //TODO: Fix audit
        //var auditRecords = HandleAudit();

        var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        //await SaveAuditRecordChanges(auditRecords);

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
                "Caller", //TODO: get caller somehow
                entry.Metadata.ClrType.Name,
                entry.Properties!.Single(p => p.Metadata!.IsPrimaryKey()).CurrentValue!.ToString()!,
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