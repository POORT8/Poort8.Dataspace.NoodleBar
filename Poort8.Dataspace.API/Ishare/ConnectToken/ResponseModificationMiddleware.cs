using System.Text;
using System.Text.Json;

namespace Poort8.Dataspace.API.Ishare.ConnectToken;

public class ResponseModificationMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    private readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.DisplayName?.Equals("HTTP: POST /api/ishare/connect/token") == true)
        {
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            responseBody.Seek(0, SeekOrigin.Begin);
            var jsonResponse = await new StreamReader(responseBody).ReadToEndAsync();

            var originalJsonDict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse);
            var updatedJsonDict = new Dictionary<string, object>
            {
                { "access_token", originalJsonDict!["accessToken"] },
                { "token_type", originalJsonDict!["tokenType"] },
                { "expires_in", originalJsonDict!["expiresIn"] }
            };
            string updatedJsonResponse = JsonSerializer.Serialize(updatedJsonDict, jsonSerializerOptions);

            var responseBytes = Encoding.UTF8.GetBytes(updatedJsonResponse);
            context.Response.Body = originalBodyStream;
            await context.Response.Body.WriteAsync(responseBytes);
        }
        else
        {
            await _next(context);
        }
    }
}
