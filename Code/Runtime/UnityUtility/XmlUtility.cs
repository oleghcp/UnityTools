using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;

namespace UU
{
    /// <summary>
    /// Utility functions to work with XML data.
    /// </summary>
    public static class XmlUtility
    {
        private static readonly XmlSerializerNamespaces NAMESPACES;

        static XmlUtility()
        {
            NAMESPACES = new XmlSerializerNamespaces(new[] { new XmlQualifiedName() });
        }

        /// <summary>
        /// Generates an XML representation of the public fields of an object.
        /// Returns the object's data in XML format.
        /// </summary>
        /// <param name="toSerialize">The object to convert to XML form.</param>
        public static string ToXml(object toSerialize)
        {
            string str;

            using (StringWriter writer = new StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(toSerialize.GetType());
                serializer.Serialize(writer, toSerialize, NAMESPACES);
                str = writer.ToString();
            }

            return str;
        }

        /// <summary>
        /// Creates an object from its XML representation.
        /// </summary>
        /// <typeparam name="T">The type of object represented by the XML.</typeparam>
        /// <param name="xml">The XML representation of the object.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FromXml<T>(string xml)
        {
            return (T)FromXml(xml, typeof(T));
        }

        /// <summary>
        /// Creates an object from its XML representation.
        /// </summary>        
        /// <param name="xml">The XML representation of the object.</param>
        /// <param name="type">The type of object represented by the XML.</param>
        public static object FromXml(string xml, Type type)
        {
            object obj;

            using (StringReader reader = new StringReader(xml))
            {
                XmlSerializer serializer = new XmlSerializer(type);
                obj = serializer.Deserialize(reader);
            }

            return obj;
        }
    }
}
