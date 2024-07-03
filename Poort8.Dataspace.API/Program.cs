//NOTE: This is scaffolding for a future API project. Now it is used by Poort8.Dataspace.CoreManager.

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();

//TODO: Add tests and exception handling