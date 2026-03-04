using System.Text.Json;
using System.Xml.Serialization;

namespace Utilities.SaveSystem;

public enum SaveFormat
{
    JSON,
    XML
}

internal static class SaveSystemHelper
{
    public static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };
    private static readonly object _formatLock = new();
    private static SaveFormat _format = SaveFormat.JSON;

    public static SaveFormat Format
    {
        get { lock (_formatLock) { return _format; } }
        set { lock (_formatLock) { _format = value; } }
    }

    public static string GetPathForFormat(Type type, SaveFormat format)
    {
        string extension = format switch
        {
            SaveFormat.JSON => "json",
            SaveFormat.XML => "xml",
            _ => "dat"
        };
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{type.Name}.{extension}");
    }

    public static string GetDefaultPath(Type type) => GetPathForFormat(type, Format);

    public static string SerializeToXml<T>(T data)
    {
        using StringWriter writer = new();
        XmlSerializer serializer = new(typeof(T));
        serializer.Serialize(writer, data);
        return writer.ToString();
    }

    public static T? DeserializeFromXml<T>(string content) where T : class
    {
        using StringReader reader = new(content);
        XmlSerializer serializer = new(typeof(T));
        return serializer.Deserialize(reader) as T;
    }
}