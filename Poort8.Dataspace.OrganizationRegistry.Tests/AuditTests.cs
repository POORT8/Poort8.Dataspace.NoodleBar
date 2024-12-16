using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Poort8.Dataspace.OrganizationRegistry.Extensions;
using System.Security.Claims;

namespace Poort8.Dataspace.OrganizationRegistry.Tests;
public class AuditTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IOrganizationRegistry _organizationRegistry;

    public AuditTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddOrganizationRegistrySqlite(options => options.ConnectionString = $"Data Source={Guid.NewGuid()}.db");

        var claims = new List<Claim>() { new(ClaimTypes.Name, "TestUser") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        serviceCollection.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor() { HttpContext = new DefaultHttpContext() { User = claimsPrincipal } });

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _organizationRegistry = _serviceProvider.GetRequiredService<IOrganizationRegistry>();

        var factory = _serviceProvider.GetRequiredService<IDbContextFactory<OrganizationContext>>();
        using var context = factory.CreateDbContext();
        context.Database.Migrate();
    }

    [Fact]
    public async Task GetAuditRecords()
    {
        var organization = OrganizationRegistryTests.CreateNewOrganization(nameof(GetAuditRecords), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);
        var auditRecords = await _organizationRegistry.GetAuditRecords();

        Assert.Single(auditRecords, a =>
            a.EntityId == organizationEntity.Identifier &&
            a.EntityType == "Organization" &&
            a.Action == "Added");
    }

    [Fact]
    public async Task UserTest()
    {
        var organization = OrganizationRegistryTests.CreateNewOrganization(nameof(UserTest), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);
        var auditRecords = await _organizationRegistry.GetAuditRecords();

        Assert.NotNull(auditRecords);
        Assert.Equal("TestUser", auditRecords[0].User);
    }
}
