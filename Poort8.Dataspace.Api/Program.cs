using Poort8.Dataspace.OrganizationRegistry;
using Poort8.Dataspace.OrganizationRegistry.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOrganizationRegistrySqlite(options => options.ConnectionString = "Data Source=OrganizationRegistry.db");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/organization", async (IOrganizationRegistry organizationRegistry) =>
{
    return await organizationRegistry.ReadOrganizations();
})
.WithName("ReadOrganizations")
.WithOpenApi();

app.MapPost("/organization", async (IOrganizationRegistry organizationRegistry, string identifier, string name) =>
{
    var organization = new Organization(identifier, name);
    return await organizationRegistry.CreateOrganization(organization);
})
.WithName("CreateOrganizations")
.WithOpenApi();

app.Run();
