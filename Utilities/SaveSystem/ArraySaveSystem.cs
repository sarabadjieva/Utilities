using System.Runtime.Serialization;
using System.Text.Json;

namespace Utilities.SaveSystem;

public static class ArraySaveSystem<T> where T : class, IExtensibleDataObject, new()
{
    public static void SetFormat(SaveFormat format) => SaveSystemHelper.Format = format;

    /// <summary>
    /// Appends a single object to the saved array or list of T.
    /// </summary>
    /// <param name="item">The object to append. Must not be null.</param>
    public static void Append(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var existing = Load();
        var updated = existing.Concat(new[] { item }).ToArray();
        Save(updated, append: false);
    }

    /// <summary>
    /// Saves an array of objects of type T to disk.
    /// </summary>
    public static void Save(T[] dataArray, bool append = true)
    {
        ArgumentNullException.ThrowIfNull(dataArray);
        string path = SaveSystemHelper.GetDefaultPath(typeof(T));
        T[] toSave = dataArray;

        if (append && File.Exists(path))
        {
            var existing = Load();
            toSave = existing.Concat(dataArray).ToArray();
        }

        string content = SaveSystemHelper.Format switch
        {
            SaveFormat.JSON => JsonSerializer.Serialize(toSave, SaveSystemHelper.JsonSerializerOptions),
            SaveFormat.XML => SaveSystemHelper.SerializeToXml(toSave),
            _ => throw new NotImplementedException()
        };
        File.WriteAllText(path, content);
    }

    /// <summary>
    /// Loads an array of objects of type T from disk.
    /// </summary>
    public static T[] Load()
    {
        try
        {
            string path = SaveSystemHelper.GetDefaultPath(typeof(T));
            if (!File.Exists(path)) return Array.Empty<T>();
            string content = File.ReadAllText(path);
            return SaveSystemHelper.Format switch
            {
                SaveFormat.JSON => JsonSerializer.Deserialize<T[]>(content) ?? Array.Empty<T>(),
                SaveFormat.XML => SaveSystemHelper.DeserializeFromXml<T[]>(content) ?? Array.Empty<T>(),
                _ => throw new NotImplementedException()
            };
        }
        catch { return Array.Empty<T>(); }
    }

    public static void DeleteAllCache()
    {
        foreach (SaveFormat format in Enum.GetValues(typeof(SaveFormat)))
        {
            string path = SaveSystemHelper.GetPathForFormat(typeof(T), format);
            if (File.Exists(path)) File.Delete(path);
        }
    }

    public static void DeleteCache(SaveFormat format)
    {
        string path = SaveSystemHelper.GetPathForFormat(typeof(T), format);
        if (File.Exists(path)) File.Delete(path);
    }
}
