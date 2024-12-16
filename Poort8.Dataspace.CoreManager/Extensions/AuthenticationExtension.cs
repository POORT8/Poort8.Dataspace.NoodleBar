using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.CoreManager.Extensions;

public static class AuthenticationExtension
{
    public static IServiceCollection AddAuthenticationSchemes(this IServiceCollection services, Action<CoreManagerOptions> coreManagerOptionsAction)
    {
        var coreManagerOptions = new CoreManagerOptions() { UseCase = "" };
        coreManagerOptionsAction?.Invoke(coreManagerOptions);

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
        })
            .AddBearerToken(AuthenticationConstants.IdentityBearer)
            .AddJwtBearer(AuthenticationConstants.Auth0Jwt, options =>
            {
                options.Authority = coreManagerOptions.JwtTokenAuthority;
                options.Audience = coreManagerOptions.JwtTokenAudience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "organizationId"
                };
            })
            .AddIdentityCookies();

        AddPolicies(services);

        return services;
    }

    private static void AddPolicies(IServiceCollection services)
    {
        //TODO: Fix roles and policies
        var readResources = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt)
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
                context.User.HasClaim(claim => claim.Type == "permission" && claim.Value == "read:resources") ||
                context.User.HasClaim(claim => claim.Type == "scope" && claim.Value == "trusted-app") ||
                context.User.IsInRole("ManageEntitiesApi"))
            .Build();
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthenticationConstants.ReadResourcesPolicy, readResources);

        var writeResources = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt)
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
                context.User.HasClaim(claim => claim.Type == "permission" && claim.Value == "write:resources") ||
                context.User.HasClaim(claim => claim.Type == "scope" && claim.Value == "trusted-app") ||
                context.User.IsInRole("ManageEntitiesApi"))
            .Build();
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthenticationConstants.WriteResourcesPolicy, writeResources);

        var deleteResources = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt)
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
                context.User.HasClaim(claim => claim.Type == "permission" && claim.Value == "delete:resources") ||
                context.User.HasClaim(claim => claim.Type == "scope" && claim.Value == "trusted-app") ||
                context.User.IsInRole("ManageEntitiesApi"))
            .Build();
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthenticationConstants.DeleteResourcesPolicy, deleteResources);

        var readPolicies = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt)
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
                context.User.HasClaim(claim => claim.Type == "permission" && claim.Value == "read:policies") ||
                context.User.HasClaim(claim => claim.Type == "scope" && claim.Value == "trusted-app") ||
                context.User.IsInRole("ManageEntitiesApi"))
            .Build();
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthenticationConstants.ReadPoliciesPolicy, readPolicies);

        var writePolicies = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt)
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
                context.User.HasClaim(claim => claim.Type == "permission" && claim.Value == "write:policies") ||
                context.User.HasClaim(claim => claim.Type == "scope" && claim.Value == "trusted-app") ||
                context.User.IsInRole("ManageEntitiesApi"))
            .Build();
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthenticationConstants.WritePoliciesPolicy, writePolicies);

        var deletePolicies = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt)
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
                context.User.HasClaim(claim => claim.Type == "permission" && claim.Value == "delete:policies") ||
                context.User.HasClaim(claim => claim.Type == "scope" && claim.Value == "trusted-app") ||
                context.User.IsInRole("ManageEntitiesApi"))
            .Build();
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthenticationConstants.DeletePoliciesPolicy, deletePolicies);

        var readOROrganizations = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt)
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
                context.User.HasClaim(claim => claim.Type == "permission" && claim.Value == "read:or-organizations") ||
                context.User.HasClaim(claim => claim.Type == "scope" && claim.Value == "trusted-app") ||
                context.User.HasClaim(claim => claim.Type == "scope" && claim.Value == "read:or-organizations") || //For Auth0 M2M Action
                context.User.IsInRole("ManageEntitiesApi"))
            .Build();
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthenticationConstants.ReadOROrganizationsPolicy, readOROrganizations);

        var readAROrganizations = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt)
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
                context.User.HasClaim(claim => claim.Type == "permission" && claim.Value == "read:ar-organizations") ||
                context.User.HasClaim(claim => claim.Type == "scope" && claim.Value == "trusted-app") ||
                context.User.IsInRole("ManageEntitiesApi"))
            .Build();
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthenticationConstants.ReadAROrganizationsPolicy, readAROrganizations);
    }
}
