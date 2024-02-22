using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Poort8.Dataspace.CoreManager.Identity.Account;
using Poort8.Dataspace.CoreManager.Identity;
using Microsoft.EntityFrameworkCore;

namespace Poort8.Dataspace.CoreManager.Extensions;

public static class IdentityExtension
{
    public static IServiceCollection AddIdentitySqlite(this IServiceCollection services)
    {
        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        }).AddIdentityCookies();

        services.AddDbContext<UserDbContext>(options =>
            options.UseSqlite("Data Source=Identity.db", builder => builder.MigrationsAssembly("Poort8.Dataspace.CoreManager")));
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<UserDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddSingleton<IEmailSender<User>, IdentityNoOpEmailSender>();

        return services;
    }
}
