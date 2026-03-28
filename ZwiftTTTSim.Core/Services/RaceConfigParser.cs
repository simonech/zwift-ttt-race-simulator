using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZwiftTTTSim.Core.Model.Config;
using ZwiftTTTSim.Core.Model.Segments;

namespace ZwiftTTTSim.Core.Services;

public class RaceConfigParser
{
    private readonly JsonSerializerOptions _jsonOptions;

    public RaceConfigParser()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new SegmentConverter() }
        };
    }

    public RaceConfig Parse(string jsonContent)
    {
        if (string.IsNullOrWhiteSpace(jsonContent))
        {
            throw new ArgumentException("JSON content cannot be null or empty.", nameof(jsonContent));
        }

        try
        {
            var config = JsonSerializer.Deserialize<RaceConfig>(jsonContent, _jsonOptions);
            
            if (config == null || config.Route == null || config.Route.Count == 0)
            {
                throw new InvalidOperationException("Parsed race configuration is invalid or contains no segments.");
            }

            return config;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse JSON configuration: {ex.Message}", ex);
        }
    }
}

public class SegmentConverter : JsonConverter<ISegment>
{
    public override ISegment? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (!root.TryGetProperty("type", out var typeProperty))
        {
            throw new JsonException("Missing 'type' property on segment definition.");
        }

        var type = typeProperty.GetString()?.ToLowerInvariant();

        return type switch
        {
            "flat" => JsonSerializer.Deserialize<FlatSegment>(root.GetRawText(), options),
            "climb" => JsonSerializer.Deserialize<ClimbSegment>(root.GetRawText(), options),
            _ => throw new JsonException($"Unknown segment type: {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, ISegment value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, options);
    }
}
