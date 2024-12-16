//NOTE: This is scaffolding for a future API project. Now it is used by Poort8.Dataspace.CoreManager.

using Azure.Monitor.OpenTelemetry.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry().UseAzureMonitor();

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();

//TODO: Add tests and exception handling