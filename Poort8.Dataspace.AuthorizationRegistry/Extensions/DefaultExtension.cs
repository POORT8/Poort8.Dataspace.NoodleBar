using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Poort8.Dataspace.AuthorizationRegistry.Extensions;
public static class DefaultExtension
{
    public static IServiceCollection AddAuthorizationRegistrySqlite(this IServiceCollection services, Action<SqliteOptions> options)
    {
        services.Configure(options);

        var sqliteOptions = new SqliteOptions();
        options?.Invoke(sqliteOptions);

        services.AddDbContextFactory<AuthorizationContext>(options => options.UseSqlite(sqliteOptions.ConnectionString));
        services.AddSingleton<IAuthorizationRegistry, AuthorizationRegistry>();
        services.AddSingleton<IRepository, Repository>();

        return services;
    }

    public class SqliteOptions
    {
        public string ConnectionString { get; set; } = "DataSource=file::memory:";
    }

    public static void RunAuthorizationRegistryMigrations(this IApplicationBuilder app)
    {
        var factory = app.ApplicationServices.GetRequiredService<IDbContextFactory<AuthorizationContext>>();

        using var context = factory.CreateDbContext();
        context.Database.Migrate();
    }
}