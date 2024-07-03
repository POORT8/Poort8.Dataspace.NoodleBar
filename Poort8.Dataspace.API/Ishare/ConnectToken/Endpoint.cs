using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using IAuthenticationService = Poort8.Ishare.Core.IAuthenticationService;

namespace Poort8.Dataspace.API.Ishare.ConnectToken;

public class Endpoint : Endpoint<Request, Results<IResult, BadRequest>>
{
    private readonly ILogger<Endpoint> _logger;

    public Endpoint(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Endpoint>();
    }

    public override void Configure()
    {
        Post("/api/ishare/connect/token");
        Options(x => x.WithTags("iSHARE"));
        AllowAnonymous();
        AllowFormData(urlEncoded: true);
    }

    public override async Task<Results<IResult, BadRequest>> ExecuteAsync(Request request, CancellationToken ct)
    {
        //TODO: Use https://fast-endpoints.com/docs/validation#application-logic-validation? Or https://fast-endpoints.com/docs/misc-conveniences#send-methods

        var authenticationService = Resolve<IAuthenticationService>();

        try
        {
            await authenticationService.ValidateClientAssertion(request.ClientAssertion, request.ClientId);
        }
        catch (Exception e)
        {
            _logger.LogInformation("Invalid client_assertion: {msg}", e.Message);
            return TypedResults.BadRequest("Invalid client_assertion.");
        }

        CreateIdentity(request.ClientId, out ClaimsPrincipal user, out AuthenticationProperties authProperties);

        _logger.LogInformation("Client {clientId} authenticated with client_assertion: {clientAssertion}", request.ClientId, request.ClientAssertion);
        return TypedResults.SignIn(user, authProperties, "Identity.Bearer");
    }

    private static void CreateIdentity(string clientId, out ClaimsPrincipal user, out AuthenticationProperties authProperties)
    {
        var claims = new Claim[]
        {
            new(ClaimTypes.Name, clientId),
            new(ClaimTypes.AuthenticationInstant, "/connect/token")
        };

        var identity = new ClaimsIdentity(claims, "Identity.Bearer");
        user = new ClaimsPrincipal(identity);
        authProperties = new AuthenticationProperties { IsPersistent = true };
    }
}
