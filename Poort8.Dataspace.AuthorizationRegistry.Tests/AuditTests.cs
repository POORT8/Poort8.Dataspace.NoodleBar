using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Poort8.Dataspace.AuthorizationRegistry.Extensions;
using Poort8.Dataspace.AuthorizationRegistry.Tests.Data;
using System.Security.Claims;

namespace Poort8.Dataspace.AuthorizationRegistry.Tests;
public class AuditTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IAuthorizationRegistry _authorizationRegisty;

    public AuditTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddAuthorizationRegistrySqlite(options => options.ConnectionString = $"Data Source={Guid.NewGuid()}.db");

        var claims = new List<Claim>() { new(ClaimTypes.Name, "TestUser") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        serviceCollection.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor() { HttpContext = new DefaultHttpContext() { User = claimsPrincipal } });

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _authorizationRegisty = _serviceProvider.GetRequiredService<IAuthorizationRegistry>();

        var factory = _serviceProvider.GetRequiredService<IDbContextFactory<AuthorizationContext>>();
        using var context = factory.CreateDbContext();
        context.Database.Migrate();
    }

    [Fact]
    public async Task GetAuditRecords()
    {
        var organization = TestData.CreateNewOrganization(nameof(GetAuditRecords), 1);
        var organizationEntity = await _authorizationRegisty.CreateOrganization(organization);
        var auditRecords = await _authorizationRegisty.GetAuditRecords();

        Assert.NotNull(auditRecords);
        Assert.Single(auditRecords.Where(a => 
            a.EntityId == organizationEntity.Identifier &&
            a.EntityType == "Organization" &&
            a.Action == "Added"));
    }

    [Fact]
    public async Task UserTest()
    {
        var organization = TestData.CreateNewOrganization(nameof(UserTest), 1);
        var organizationEntity = await _authorizationRegisty.CreateOrganization(organization);
        var auditRecords = await _authorizationRegisty.GetAuditRecords();

        Assert.NotNull(auditRecords);
        Assert.Equal("TestUser", auditRecords[0].User);
    }
}
