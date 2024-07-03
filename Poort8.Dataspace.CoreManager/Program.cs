using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.FeatureManagement;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.API;
using Poort8.Dataspace.API.Ishare.ConnectToken;
using Poort8.Dataspace.AuthorizationRegistry.Extensions;
using Poort8.Dataspace.CoreManager;
using Poort8.Dataspace.CoreManager.Extensions;
using Poort8.Dataspace.CoreManager.Layout;
using Poort8.Dataspace.CoreManager.Services;
using Poort8.Dataspace.Identity;
using Poort8.Dataspace.OrganizationRegistry.Extensions;
using Poort8.Ishare.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options => options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromHours(1));
builder.Services.AddFluentUIComponents();

if (builder.Configuration.GetValue<bool>("UseSqlite"))
{
    builder.Services.AddOrganizationRegistrySqlite(options => options.ConnectionString = "Data Source=OrganizationRegistry.db");
    builder.Services.AddAuthorizationRegistrySqlite(options => options.ConnectionString = "Data Source=AuthorizationRegistry.db");
    builder.Services.AddIdentitySqlite(options => options.ConnectionString = "Data Source=Identity.db");
}
else
{
    builder.Services.AddOrganizationRegistrySqlServer(options => options.ConnectionString = builder.Configuration["OrganizationRegistry:ConnectionString"]);
    builder.Services.AddAuthorizationRegistrySqlServer(options => options.ConnectionString = builder.Configuration["AuthorizationRegistry:ConnectionString"]);
    builder.Services.AddIdentitySqlServer(options => options.ConnectionString = builder.Configuration["Identity:ConnectionString"]);
}

builder.Services.AddFeatureManagement();
builder.Services.AddOptions<CoreManagerOptions>()
    .Bind(builder.Configuration.GetSection(CoreManagerOptions.Section))
    .ValidateDataAnnotations()
    .ValidateOnStart();

bool ishareEnabled = builder.Configuration.GetSection("FeatureManagement").GetValue<bool>("IshareEnabled");
if (ishareEnabled) builder.Services.AddIshareCoreServices(builder.Configuration);

builder.Services.AddScoped<StateContainer>();

builder.Services.AddAuthenticationSchemes(options =>
{
    options.JwtTokenAuthority = builder.Configuration["CoreManagerOptions:JwtTokenAuthority"];
    options.JwtTokenAudience = builder.Configuration["CoreManagerOptions:JwtTokenAudience"];
});

//API
builder.Services.AddFastEndpoints(o => o.Assemblies = [typeof(Poort8.Dataspace.API.Ishare.ConnectToken.Endpoint).Assembly])
    .SwaggerDocument(options =>
    {
        options.AutoTagPathSegmentIndex = 0;
        options.EndpointFilter = ep => !(ep.EndpointTags?.Contains("ExcludeFromSwagger") ?? false);
    });
builder.Services.AddHealthChecks();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.RunOrganizationRegistryMigrations();
app.RunAuthorizationRegistryMigrations();
app.RunIdentityMigrations();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

var featureManager = app.Services.GetRequiredService<IFeatureManager>();
var apiDisabled = await featureManager.IsEnabledAsync(FeatureManagement.ApiDisabled);
if (!apiDisabled)
{
    app.UseFastEndpoints(c =>
    {
        EndpointsConfiguration.Filter(c, ishareEnabled);
        EndpointsConfiguration.ConfigureProcessors(c);
    }).UseSwaggerGen();
}

app.UseMiddleware<ResponseModificationMiddleware>();

app.MapHealthChecks("/health");
app.MapIdentityApi<User>();
app.MapAdditionalIdentityEndpoints();

app.SetUserRoles();

app.Run();

public partial class Program { } //So that the ApiApp test fixture can find this class
