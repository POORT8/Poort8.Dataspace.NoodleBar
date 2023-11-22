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

        //NOTE: Use builder.MigrationsAssembly("Poort8.Dataspace.CoreManager") to create migrations and move them to the right project

        services.AddDbContextFactory<AuthorizationContext>(
            options => options.UseSqlite(sqliteOptions.ConnectionString,
            builder => builder.MigrationsAssembly("Poort8.Dataspace.AuthorizationRegistry.SqliteMigrations")));
        services.AddSingleton<IAuthorizationRegistry, AuthorizationRegistry>();
        services.AddSingleton<IRepository, Repository>();

        //NOTE: The audit uses IHttpContextAccessor to get the user.

        return services;
    }

    public class SqliteOptions
    {
        public string ConnectionString { get; set; } = "DataSource=file::memory:";
    }

    public static IServiceCollection AddAuthorizationRegistrySqlServer(this IServiceCollection services, Action<SqlServerOptions> options)
    {
        services.Configure(options);

        var sqlServerOptions = new SqlServerOptions();
        options?.Invoke(sqlServerOptions);

        services.AddDbContextFactory<AuthorizationContext>(
            options => options.UseSqlServer(sqlServerOptions.ConnectionString,
            builder => builder.MigrationsAssembly("Poort8.Dataspace.AuthorizationRegistry.SqlServerMigrations")));
        services.AddSingleton<IAuthorizationRegistry, AuthorizationRegistry>();
        services.AddSingleton<IRepository, Repository>();

        //NOTE: The audit uses IHttpContextAccessor to get the user.

        return services;
    }

    public class SqlServerOptions
    {
        public string? ConnectionString { get; set; }
    }

    public static void RunAuthorizationRegistryMigrations(this IApplicationBuilder app)
    {
        var factory = app.ApplicationServices.GetRequiredService<IDbContextFactory<AuthorizationContext>>();

        using var context = factory.CreateDbContext();
        context.Database.Migrate();
    }
}