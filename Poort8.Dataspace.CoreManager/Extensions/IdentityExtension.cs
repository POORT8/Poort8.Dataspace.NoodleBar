using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Poort8.Dataspace.CoreManager.Identity.Account;
using Poort8.Dataspace.CoreManager.Services;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.CoreManager.Extensions;

public static class IdentityExtension
{
    public static IServiceCollection AddIdentitySqlite(this IServiceCollection services, Action<SqliteOptions> options)
    {
        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        var sqliteOptions = new SqliteOptions();
        options?.Invoke(sqliteOptions);

        //NOTE: Use builder.MigrationsAssembly("Poort8.Dataspace.CoreManager") to create migrations and move them to the right project

        services.AddDbContext<UserDbContext>(options =>
            options.UseSqlite(sqliteOptions.ConnectionString, builder => builder.MigrationsAssembly("Poort8.Dataspace.Identity.SqliteMigrations")));
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<UserDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddSingleton<IEmailSender<User>, EmailSender>();

        return services;
    }

    public class SqliteOptions
    {
        public string ConnectionString { get; set; } = "DataSource=file::memory:";
    }

    public static IServiceCollection AddIdentitySqlServer(this IServiceCollection services, Action<SqlServerOptions> options)
    {
        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        var sqlServerOptions = new SqlServerOptions();
        options?.Invoke(sqlServerOptions);

        //NOTE: Use builder.MigrationsAssembly("Poort8.Dataspace.CoreManager") to create migrations and move them to the right project

        services.AddDbContext<UserDbContext>(options =>
            options.UseSqlServer(sqlServerOptions.ConnectionString, builder => builder.MigrationsAssembly("Poort8.Dataspace.Identity.SqlServerMigrations")));
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<UserDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddSingleton<IEmailSender<User>, EmailSender>();

        return services;
    }

    public class SqlServerOptions
    {
        public string? ConnectionString { get; set; }
    }

    public static void RunIdentityMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        context.Database.Migrate();
    }
}
