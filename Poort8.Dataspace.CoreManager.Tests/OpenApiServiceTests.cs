using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Poort8.Dataspace.CoreManager.Services;
using static Poort8.Dataspace.CoreManager.Catalog.Index;

namespace Poort8.Dataspace.CoreManager.Tests;

public class OpenApiServiceTests
{
    private const string _sampleUrl = "https://petstore3.swagger.io/api/v3/openapi.json";
    private readonly OpenApiService _openApiService;

    public OpenApiServiceTests()
    {
        var logger = Substitute.For<ILogger<UseCaseService>>();
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient().Returns(new HttpClient());

        var services = new ServiceCollection();
#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services.AddHybridCache();
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        var serviceProvider = services.BuildServiceProvider();
        var cache = serviceProvider.GetRequiredService<HybridCache>();

        _openApiService = new OpenApiService(logger, httpClientFactory, cache);
    }

    [Fact]
    public async Task GetOpenApiDefinition()
    {
        var result = await _openApiService.GetOpenApiDefinition(_sampleUrl);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetDataSourceViewModel()
    {
        var result = await _openApiService.GetDataSourceViewModel(_sampleUrl, "organization", "logoUrl");

        Assert.NotNull(result);
        Assert.IsType<DataSourceViewModel>(result);
    }

    [Fact]
    public async Task GetOpenApiDefinitionJson()
    {
        var result = await _openApiService.GetOpenApiDefinitionJson(_sampleUrl);

        Assert.NotNull(result);
    }
}
