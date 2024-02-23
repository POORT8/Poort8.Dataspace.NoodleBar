using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry.Extensions;
using Poort8.Dataspace.CoreManager.API;
using Poort8.Dataspace.CoreManager.Extensions;
using Poort8.Dataspace.CoreManager.Layout;
using Poort8.Dataspace.CoreManager.Services;
using Poort8.Dataspace.OrganizationRegistry.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents();

//Choose between Sqlite or SqlServer
builder.Services.AddOrganizationRegistrySqlite(options => options.ConnectionString = "Data Source=OrganizationRegistry.db");
builder.Services.AddAuthorizationRegistrySqlite(options => options.ConnectionString = "Data Source=AuthorizationRegistry.db");
builder.Services.AddIdentitySqlite();

//builder.Services.AddOrganizationRegistrySqlServer(options => options.ConnectionString = builder.Configuration["OrganizationRegistry:ConnectionString"]);
//builder.Services.AddAuthorizationRegistrySqlServer(options => options.ConnectionString = builder.Configuration["AuthorizationRegistry:ConnectionString"]);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<StateContainer>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.RunOrganizationRegistryMigrations();
app.RunAuthorizationRegistryMigrations();
app.RunIdentityMigrations();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapFeatureEndpoints();
app.MapAuthorizationEndpoints();
app.MapHealthChecks("/health");

app.MapAdditionalIdentityEndpoints();

app.Run();
