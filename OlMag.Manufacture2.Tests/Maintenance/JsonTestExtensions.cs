using System.Text.Json.Serialization;
using System.Text.Json;

namespace OlMag.Manufacture2.Tests.Maintenance;

public static class JsonTestExtensions
{
    public static readonly JsonSerializerOptions JsonSerializerOptions;

    static JsonTestExtensions()
    {
        JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public static T? Deserialize<T>(string source)
    {
        return JsonSerializer.Deserialize<T>(source, JsonSerializerOptions);
    }
}