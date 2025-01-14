using System.Text.Json;
using System.Text.Json.Serialization;

namespace OlMag.Manufacture2.Helpers;

public static class SerializerOptionsHelper
{
    public static JsonSerializerOptions SerializerOptions =>
        new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };
}