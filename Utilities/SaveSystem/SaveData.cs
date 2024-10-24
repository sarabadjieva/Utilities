using System.Runtime.Serialization;

namespace Utilities.SaveSystem
{
    [DataContract]
    internal class SaveData : IExtensibleDataObject
    {
        public ExtensionDataObject? ExtensionData { get; set; }

        public Type[] KnownTypes { get; }

        public SaveData()
        {
            KnownTypes = GetType()
                .GetProperties()
                .Where(p => p.GetCustomAttributes(true).Any(attr => attr is DataMemberAttribute))
                .Select(p => p.GetType())
                .ToArray();
        }
    }
}
