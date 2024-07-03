using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Poort8.Dataspace.API.Tests;

public class ApiApp : AppFixture<Program>
{
    protected override Task SetupAsync()
    {
        return Task.CompletedTask;
    }

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        builder.UseConfiguration(new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
            .Build());
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
    }

    protected override Task TearDownAsync()
    {
        return Task.CompletedTask;
    }
}
