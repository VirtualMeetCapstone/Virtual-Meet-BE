using System.Text.Json;

namespace GOCAP.Common;

public static class JsonHelper
{
    /// <summary>
    /// Parse object to JSON string.
    /// </summary>
    public static string Serialize<T>(T? obj)
    {
        return obj is null ? string.Empty : JsonSerializer.Serialize(obj);
    }

    /// <summary>
    /// Parse JSON string to object.
    /// </summary>
    public static T? Deserialize<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    /// <summary>
    /// Check whether json string is valid or not.
    /// </summary>
    public static bool IsValidJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;
        try
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            return doc.RootElement.ValueKind != JsonValueKind.Undefined;
        }
        catch
        {
            return false;
        }
    }
}