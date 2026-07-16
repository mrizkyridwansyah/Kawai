using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kawai.Domain.Shared;

public class JsonTrimString : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Begitu dibaca dari JSON client, langsung di-trim
        return reader.GetString()?.Trim();
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.Trim());
    }
}