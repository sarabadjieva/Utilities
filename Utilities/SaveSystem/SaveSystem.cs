using System.Runtime.Serialization;
using System.Xml;

namespace Utilities.SaveSystem
{
    public class SaveSystem<T> where T : SaveData
    {
        private const string SAVE_EXT = ".txt";
        private readonly string _saveFile;

        public SaveSystem()
        {
            _saveFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "save" + SAVE_EXT);
        }

        public void Save(T data)
        {
            using (FileStream fs = new(_saveFile, FileMode.OpenOrCreate))
            {
                XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs);

                DataContractSerializer serializer = new(data.GetType(), knownTypes: data.KnownTypes);
                serializer.WriteObject(writer, data);

                writer.Close();
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public T? Load()
        {
            if (!File.Exists(_saveFile))
                return null;

            using (FileStream fs = new(_saveFile, FileMode.Open))
            {
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                
                var serializer = new DataContractSerializer(typeof(T));
                T? data = serializer.ReadObject(reader) as T;
                
                reader.Close();

                return data;
            }
        }
    }
}
