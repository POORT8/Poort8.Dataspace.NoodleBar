using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Hybrid;
using NSwag;
using static Poort8.Dataspace.CoreManager.Catalog.Index;

namespace Poort8.Dataspace.CoreManager.Services;

public class OpenApiService
{
    private readonly ILogger<UseCaseService> _logger;
    private readonly HttpClient _httpClient;
    private readonly HybridCache _cache;

    public OpenApiService(
        ILogger<UseCaseService> logger,
        IHttpClientFactory httpClientFactory,
        HybridCache cache)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _cache = cache;
    }

    public async Task<OpenApiDocument> GetOpenApiDefinition(string url, CancellationToken token = default)
    {
        var json = await _cache.GetOrCreateAsync(
            $"OpenApiJson-{Uri.EscapeDataString(url)}",
            async cancel => await _httpClient.GetStringAsync(url, cancel),
            new HybridCacheEntryOptions() { Expiration = TimeSpan.FromMinutes(5) },
            cancellationToken: token
        );

        return await ParseOpenApiDocument(json, token);
    }

    private static async Task<OpenApiDocument> ParseOpenApiDocument(string json, CancellationToken cancellationToken)
    {
        try
        {
            var openApiDocument = await OpenApiDocument.FromJsonAsync(json, cancellationToken);
            return openApiDocument;
        }
        catch (Exception)
        {
            var document = await OpenApiYamlDocument.FromYamlAsync(json, cancellationToken);
            var openApiDocument = await OpenApiDocument.FromJsonAsync(document.ToJson(), cancellationToken);
            return openApiDocument;
        }
    }

    public async Task<string> GetOpenApiDefinitionJson(string url)
    {
        var openApiDocument = await GetOpenApiDefinition(url);
        return openApiDocument.ToJson();
    }

    public async Task<string> GetOpenApiDefinitionServer(string url)
    {
        var openApiDocument = await GetOpenApiDefinition(url);

        if (Uri.TryCreate(url, UriKind.Absolute, out var host))
        {
            var serverUrl = openApiDocument.Servers.FirstOrDefault()?.Url;
            if (serverUrl != null)
            {
                if (Uri.TryCreate(serverUrl, UriKind.Absolute, out var absoluteServerUri))
                {
                    return absoluteServerUri.ToString();
                }
                else if (Uri.TryCreate(serverUrl, UriKind.Relative, out var relativeServerUri))
                {
                    return new Uri(host, relativeServerUri).ToString();
                }
                else
                {
                    _logger.LogWarning("P8.warn - Invalid server URL in the OpenAPI document for {openApiUrl}", url);
                    return $"{host.Scheme}://{host.Host}";
                }
            }
            else
            {
                _logger.LogWarning("P8.warn - No servers defined in the OpenAPI document for {openApiUrl}", url);
                return $"{host.Scheme}://{host.Host}";
            }
        }
        else
        {
            _logger.LogError("P8.err - Invalid URL: {openApiUrl}", url);
            return "Invalid URL";
        }
    }

    public async Task<DataSourceViewModel?> GetDataSourceViewModel(string url, string organization, string? logoUrl)
    {
        try
        {
            var openApi = await GetOpenApiDefinition(url);

            Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var host);

            return new DataSourceViewModel()
            {
                Type = "Schema: " + openApi.SchemaType,
                LogoUrl = logoUrl,
                Organization = organization,
                Title = openApi.Info.Title,
                Description = openApi.Info.Description,
                DescriptionMarkdown = new MarkupString(Markdown.ToHtml(openApi.Info.Description ?? string.Empty)),
                Version = openApi.Info.Version,
                Swagger = url,
                Terms = openApi.Info.TermsOfService,
                License = openApi.Info.License?.Url,
                Contact = string.IsNullOrEmpty(openApi.Info.Contact?.Email) ? null : "mailto:" + openApi.Info.Contact?.Email,
                Docs = openApi.ExternalDocumentation?.Url,
                Host = host?.Host ?? "Invalid URL",
                Url = url
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "P8.err - Error reading the OpenAPI definition");
            return null;
        }
    }
}
