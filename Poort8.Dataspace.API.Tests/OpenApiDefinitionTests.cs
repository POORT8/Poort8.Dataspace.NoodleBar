using FastEndpoints.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation;
using Snapshooter.Xunit;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

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

        var serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var jsonString = orderedApiDefinition!.ToJsonString(serializerOptions);

        Snapshot.Match(jsonString
            .Replace("\r\n", "\n")
            .Replace("\\r\\n", "\\n"));
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
