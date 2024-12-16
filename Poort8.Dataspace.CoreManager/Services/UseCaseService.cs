using Microsoft.Extensions.Options;
using NSwag;

namespace Poort8.Dataspace.CoreManager.Services;

public class UseCaseService
{
    private readonly ILogger<UseCaseService> _logger;
    private readonly HttpClient _httpClient;

    public List<UseCase> UseCases { get; set; } = new List<UseCase>();

    public UseCaseService(
        ILogger<UseCaseService> logger,
        IOptions<CoreManagerOptions> options,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();

        AuthorizationRegistry.UseCases.AuthorizationModels.Keys.ToList().ForEach(key =>
        {
            UseCases.Add(new UseCase(key));
        });

        var openApiUrls = options.Value.OpenApiUrls;
        if (openApiUrls != null)
        {
            foreach (var url in openApiUrls.Split(";"))
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out _))
                    GetUseCaseFromOpenApi(url);
            }
        }
    }

    private async void GetUseCaseFromOpenApi(string url)
    {
        try
        {
            var openApiJson = await _httpClient.GetStringAsync(url);
            var openApi = await OpenApiDocument.FromJsonAsync(openApiJson);

            foreach (var path in openApi.Paths)
            {
                foreach (var operation in path.Value)
                {
                    foreach (var tag in operation.Value.Tags)
                    {
                        if (UseCases.Any(x => x.Name.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                        {
                            UseCases.First(x => x.Name.Equals(tag, StringComparison.OrdinalIgnoreCase)).Actions.Add(operation.Key.ToString());
                            UseCases.First(x => x.Name.Equals(tag, StringComparison.OrdinalIgnoreCase)).ResourceTypes.Add(operation.Value.OperationId);
                        }
                        else
                        {
                            UseCases.Add(new UseCase(tag.ToLower())
                            {
                                Actions = [operation.Key.ToString()],
                                ResourceTypes = [operation.Value.OperationId]
                            });
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError("P8.err - Error while getting use cases from OpenAPI: {msg}", e.Message);
        }
    }
}

public class UseCase
{
    public string Name { get; set; }
    public List<string> Actions { get; set; } = new();
    public List<string> ResourceTypes { get; set; } = new();
    public List<string> ResourceIdentifiers { get; set; } = new();
    public List<string> ResourceAttributes { get; set; } = new();

    public UseCase(string name)
    {
        Name = name;
    }
}
