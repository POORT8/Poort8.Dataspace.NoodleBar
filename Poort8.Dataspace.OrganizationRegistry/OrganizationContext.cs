using Microsoft.EntityFrameworkCore;

namespace Poort8.Dataspace.OrganizationRegistry;
public class OrganizationContext : DbContext
{
    public DbSet<Organization> Organizations { get; set; }

    public OrganizationContext(DbContextOptions<OrganizationContext> options) : base(options)
    {
    }
}