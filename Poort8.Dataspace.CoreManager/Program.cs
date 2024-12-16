using Azure.Monitor.OpenTelemetry.AspNetCore;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.FeatureManagement;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.API;
using Poort8.Dataspace.API.Extensions;
using Poort8.Dataspace.API.Ishare.ConnectToken;
using Poort8.Dataspace.AuthorizationRegistry.Extensions;
using Poort8.Dataspace.CoreManager;
using Poort8.Dataspace.CoreManager.Extensions;
using Poort8.Dataspace.CoreManager.Layout;
using Poort8.Dataspace.CoreManager.Services;
using Poort8.Dataspace.Identity;
using Poort8.Dataspace.OrganizationRegistry.Extensions;
using Poort8.Ishare.Core;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

if (!string.IsNullOrEmpty(builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING")))
    builder.Services.AddOpenTelemetry().UseAzureMonitor();

builder.Services.AddHttpClient();

#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

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
builder.Services.AddScoped<UseCaseService>();
builder.Services.AddScoped<OpenApiService>();

builder.Services.AddAuthenticationSchemes(options =>
{
    options.JwtTokenAuthority = builder.Configuration["CoreManagerOptions:JwtTokenAuthority"];
    options.JwtTokenAudience = builder.Configuration["CoreManagerOptions:JwtTokenAudience"];
});

//API
builder.Services.AddFastEndpoints(o => o.Assemblies = [typeof(Poort8.Dataspace.API.Ishare.ConnectToken.Endpoint).Assembly]);
builder.Services.AddApiDefinition();
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

app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

var featureManager = app.Services.GetRequiredService<IFeatureManager>();
var apiDisabled = await featureManager.IsEnabledAsync(FeatureManagement.ApiDisabled);
if (!apiDisabled)
{
    app.UseDefaultExceptionHandler().UseFastEndpoints(c =>
    {
        EndpointsConfiguration.Filter(c, ishareEnabled);
        EndpointsConfiguration.ConfigureProcessors(c);
    }).UseSwaggerGen(); //Swagger

    app.UseOpenApi(options => options.Path = "/openapi/{documentName}.json"); //Scalar
    app.MapScalarApiReference(); //Scalar
}

app.UseMiddleware<ResponseModificationMiddleware>();

app.MapHealthChecks("/health");
app.MapIdentityApi<User>();
app.MapAdditionalIdentityEndpoints();

app.MapApiReferenceEndpoints();

var registerDisabled = await featureManager.IsEnabledAsync(FeatureManagement.RegisterNewUsersDisabled);
if (registerDisabled)
{
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/register"))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Register endpoint is disabled.");
        }
        else
        {
            await next.Invoke();
        }
    });
}

app.SetUserRoles();

app.Run();

public partial class Program { } //So that the ApiApp test fixture can find this class
