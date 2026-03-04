using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;

namespace Utilities.SaveSystem
{
    public static class SingleSaveSystem<T> where T : class, IExtensibleDataObject, new()
    {
        public static void SetFormat(SaveFormat format) => SaveSystemHelper.Format = format;

        public static void Save(T data)
        {
            ArgumentNullException.ThrowIfNull(data);
            string content = SaveSystemHelper.Format switch
            {
                SaveFormat.JSON => JsonSerializer.Serialize(data, SaveSystemHelper.JsonSerializerOptions),
                SaveFormat.XML => SaveSystemHelper.SerializeToXml(data),
                _ => throw new NotImplementedException()
            };
            File.WriteAllText(SaveSystemHelper.GetDefaultPath(typeof(T)), content);
        }

        public static T Load()
        {
            try
            {
                string path = SaveSystemHelper.GetDefaultPath(typeof(T));
                if (!File.Exists(path)) return new T();
                string content = File.ReadAllText(path);
                return SaveSystemHelper.Format switch
                {
                    SaveFormat.JSON => JsonSerializer.Deserialize<T>(content) ?? new T(),
                    SaveFormat.XML => SaveSystemHelper.DeserializeFromXml<T>(content) ?? new T(),
                    _ => throw new NotImplementedException()
                };
            }
            catch { return new T(); }
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
}