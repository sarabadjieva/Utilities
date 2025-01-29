using System.Runtime.Serialization;
using System.Text.Json;
using System.Xml.Serialization;

namespace Utilities
{
    public enum SaveFormat { JSON, XAML }

    public static class SaveSystem<T> where T : class, IExtensibleDataObject, new()
    {
        private static readonly string _defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{typeof(T).Name}.json");
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

        private static SaveFormat _format = SaveFormat.JSON;

        public static void SetFormat(SaveFormat format) => _format = format;

        /// <summary>
        /// Saves a single object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void Save(T data)
        {
            ArgumentNullException.ThrowIfNull(data);

            string content = _format switch
            {
                SaveFormat.JSON => JsonSerializer.Serialize(data, _jsonSerializerOptions),
                SaveFormat.XAML => SerializeToXaml(data),
                _ => throw new NotImplementedException()
            };

            File.WriteAllText(_defaultPath, content);
        }

        /// <summary>
        /// Saves an array of objects.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void Save(T[] dataArray)
        {
            ArgumentNullException.ThrowIfNull(dataArray);

            string content = _format switch
            {
                SaveFormat.JSON => JsonSerializer.Serialize(dataArray, _jsonSerializerOptions),
                SaveFormat.XAML => SerializeToXaml(dataArray),
                _ => throw new NotImplementedException()
            };

            File.WriteAllText(_defaultPath, content);
        }

        /// <summary>
        /// Loads a single object.
        /// </summary>
        public static T Load()
        {
            if (!File.Exists(_defaultPath)) return new T();

            string content = File.ReadAllText(_defaultPath);
            return _format switch
            {
                SaveFormat.JSON => JsonSerializer.Deserialize<T>(content) ?? new T(),
                SaveFormat.XAML => DeserializeFromXaml<T>(content) ?? new T(),
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Loads an array of objects.
        /// </summary>
        public static T[] LoadArray()
        {
            if (!File.Exists(_defaultPath)) return Array.Empty<T>();

            string content = File.ReadAllText(_defaultPath);
            return _format switch
            {
                SaveFormat.JSON => JsonSerializer.Deserialize<T[]>(content) ?? Array.Empty<T>(),
                SaveFormat.XAML => DeserializeFromXaml<T[]>(content) ?? Array.Empty<T>(),
                _ => throw new NotImplementedException()
            };
        }

        // XAML Serialization
        // Type argument can be an array of objects.
        private static string SerializeToXaml<U>(U data)
        {
            using StringWriter writer = new();
            XmlSerializer serializer = new(typeof(U));
            serializer.Serialize(writer, data);
            return writer.ToString();
        }

        // XAML Deserialization
        // Type argument can be an array of objects.
        private static U? DeserializeFromXaml<U>(string content) where U : class
        {
            using StringReader reader = new(content);
            XmlSerializer serializer = new(typeof(U));
            return serializer.Deserialize(reader) as U;
        }
    }
}