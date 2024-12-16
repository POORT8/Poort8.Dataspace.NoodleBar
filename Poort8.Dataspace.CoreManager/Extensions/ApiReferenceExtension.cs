using Poort8.Dataspace.CoreManager.Services;
using System.Text.Json;

namespace Poort8.Dataspace.CoreManager.Extensions;

public static class ApiReferenceExtension
{
    public static void MapApiReferenceEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/openapi", async (string url, OpenApiService openApiService) =>
        {
            var openApiJson = await openApiService.GetOpenApiDefinitionJson(url);
            return Results.Content(openApiJson, "application/json");
        });

        app.MapGet("/api-reference", async (HttpContext context, string url, OpenApiService openApiService) =>
        {
            var config = new
            {
                spec = new { url = "openapi?url=" + url },
                servers = new[] { new { url = await openApiService.GetOpenApiDefinitionServer(url) } }
            };
            var configJson = JsonSerializer.Serialize(config);

            var htmlContent = $"""
            <!doctype html>
            <html>
            <head>
                <title>API Reference</title>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1" />
            </head>
            <body>
                <script id="api-reference"></script>
                <script>
                    const config = {configJson};
                    const apiReferenceScript = document.getElementById('api-reference');
                    apiReferenceScript.dataset.configuration = JSON.stringify(config);
                </script>
                <script src="https://unpkg.com/@scalar/api-reference"></script>
            </body>
            </html>
            """;

            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(htmlContent);
        });
    }
}
