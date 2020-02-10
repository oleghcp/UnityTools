using System;

namespace UU.SaveLoad.SaveProviderStuff
{
    /// <summary>
    /// A serializer for SaveProvider (UU.XmlUtility wrapper).
    /// </summary>
    public sealed class XMLSerializer : Serializer
    {
        public string Serialize(object toSerialize)
        {
            return XmlUtility.ToXml(toSerialize);
        }

        public object Deserialize(string toDeserialize, Type type)
        {
            return XmlUtility.FromXml(toDeserialize, type);
        }
    }
}
