﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Poort8.Dataspace.OrganizationRegistry.Extensions;
public static class DefaultExtension
{
    public static IServiceCollection AddOrganizationRegistrySqlite(this IServiceCollection services, Action<SqliteOptions> options)
    {
        services.Configure(options);

        var sqliteOptions = new SqliteOptions();
        options?.Invoke(sqliteOptions);

        //NOTE: Use builder.MigrationsAssembly("Poort8.Dataspace.CoreManager") to create migrations and move them to the right project

        services.AddDbContextFactory<OrganizationContext>(
            options => options.UseSqlite(sqliteOptions.ConnectionString,
            builder => builder.MigrationsAssembly("Poort8.Dataspace.OrganizationRegistry.SqliteMigrations")));
        services.AddSingleton<IOrganizationRegistry, OrganizationRegistry>();

        //NOTE: The audit uses IHttpContextAccessor to get the user.

        return services;
    }

    public class SqliteOptions
    {
        public string ConnectionString { get; set; } = "DataSource=file::memory:";
    }

    public static IServiceCollection AddOrganizationRegistrySqlServer(this IServiceCollection services, Action<SqlServerOptions> options)
    {
        services.Configure(options);

        var sqlServerOptions = new SqlServerOptions();
        options?.Invoke(sqlServerOptions);

        services.AddDbContextFactory<OrganizationContext>(
            options => options.UseSqlServer(sqlServerOptions.ConnectionString,
            builder => builder.MigrationsAssembly("Poort8.Dataspace.OrganizationRegistry.SqlServerMigrations")));
        services.AddSingleton<IOrganizationRegistry, OrganizationRegistry>();

        //NOTE: The audit uses IHttpContextAccessor to get the user.

        return services;
    }

    public class SqlServerOptions
    {
        public string? ConnectionString { get; set; }
    }

    public static void RunOrganizationRegistryMigrations(this IApplicationBuilder app)
    {
        var factory = app.ApplicationServices.GetRequiredService<IDbContextFactory<OrganizationContext>>();

        using var context = factory.CreateDbContext();
        context.Database.Migrate();
    }
}
