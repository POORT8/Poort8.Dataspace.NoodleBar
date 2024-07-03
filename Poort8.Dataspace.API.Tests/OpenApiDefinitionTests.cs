using FastEndpoints.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation;
using Snapshooter.Xunit;
using System.Text.Json.Nodes;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;

namespace Poort8.Dataspace.API.Tests;

public class OpenApiDefinitionTests(ApiApp App) : TestBase<ApiApp>
{
    [Fact]
    public async Task TestApiDefinitionChangedUsingDoc()
    {
        var docGenerator = App.Services.GetRequiredService<IOpenApiDocumentGenerator>();
        var apiDefinition = await docGenerator.GenerateAsync("v1");

        var jsonNode = JsonNode.Parse(apiDefinition.ToJson());
        var orderedApiDefinition = OrderApiDefinition(jsonNode!);

        try
        {
            Snapshot.Match(orderedApiDefinition);
        }
        catch (Exception)
        {
            //On GitHub workflow Snapshot.Match fails, so try again with FluentAssertions
            var snapshot = await File.ReadAllTextAsync("__snapshots__/OpenApiDefinitionTests.TestApiDefinitionChangedUsingDoc.snap");
            var snapshotDoc = JToken.Parse(snapshot);
            var apiDefinitionDoc = JToken.Parse(apiDefinition.ToJson());
            apiDefinitionDoc.Should().BeEquivalentTo(snapshotDoc);
        }
    }

    private static JsonNode? OrderApiDefinition(JsonNode? jsonNode)
    {
        if (jsonNode is JsonObject jsonObject)
        {
            var orderedObject = new JsonObject();
            foreach (var property in jsonObject.OrderBy(p => p.Key))
            {
                orderedObject[property.Key] = OrderApiDefinition(property.Value?.DeepClone());
            }
            return orderedObject;
        }
        else if (jsonNode is JsonArray jsonArray)
        {
            var orderedArray = new JsonArray(jsonArray.Select(node => OrderApiDefinition(node?.DeepClone())).ToArray());
            return orderedArray;
        }
        else
        {
            return jsonNode;
        }
    }
}
