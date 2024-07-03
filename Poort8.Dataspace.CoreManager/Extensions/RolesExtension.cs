using Microsoft.AspNetCore.Identity;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.CoreManager.Extensions;

public static class RolesExtension
{
    //TODO: MVP implematation
    public static void SetUserRoles(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("RolesExtension");

        string[] roleNames = ["ManageEntitiesApi", "CanSetPolicyIssuer"];

        foreach (var role in roleNames)
        {
            var roleExist = roleManager.RoleExistsAsync(role).GetAwaiter().GetResult();
            if (!roleExist)
            {
                var result = roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
                if (result.Succeeded)
                    logger.LogInformation("P8.inf - Added ManageEntitiesApi role");
                else
                    logger.LogError("P8.err - Failed to add ManageEntitiesApi role");
            }

            var roles = config.GetSection("Roles").GetValue<string>(role);
            var userRoles = roles?.Split(',');

            foreach (var userRole in userRoles ?? [])
            {
                var user = userManager.FindByEmailAsync(userRole).GetAwaiter().GetResult();
                if (user != null)
                {
                    var result = userManager.AddToRoleAsync(user, role).GetAwaiter().GetResult();
                    if (result.Succeeded)
                        logger.LogInformation("P8.inf - Added ManageEntitiesApi role to {user}", user.Email);
                    else if (result.Errors.Count() == 1 && result.Errors.First().Code != "UserAlreadyInRole")
                        logger.LogError("P8.err - Failed to add ManageEntitiesApi role to {user}", user.Email);
                }
            }
        }
    }
}
