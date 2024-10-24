using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Utilities.SaveSystem
{
    internal class SaveSystem
    {
        private const string SAVE_EXT = ".txt";
        private readonly string _saveFile;

        public SaveSystem()
        {
            _saveFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "save", SAVE_EXT);

        }

        public void Save(SaveData data)
        {
            FileStream fs = new(_saveFile, FileMode.OpenOrCreate);
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs);

            DataContractSerializer serializer = new(data.GetType(), knownTypes: data.KnownTypes);
            serializer.WriteObject(writer, data);

            writer.Close();
            fs.Close();
        }

        public SaveData Load()
        {
            return null;
        }
    }
}
