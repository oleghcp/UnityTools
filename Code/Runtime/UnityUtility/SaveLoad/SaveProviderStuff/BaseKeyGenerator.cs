using System;
using System.Text;

namespace UU.SaveLoad.SaveProviderStuff
{
    public class BaseKeyGenerator : KeyGenerator
    {
        private const char SEP = '.';
        private string m_keyExt;

        private StringBuilder m_builder;

        public BaseKeyGenerator()
        {
            m_builder = new StringBuilder();
        }

        public BaseKeyGenerator(string keyExtension) : this()
        {
            m_keyExt = keyExtension;
        }

        public string Generate(Type objectType, string fieldName, string objectID)
        {
            m_builder.Clear()
                     .Append(objectType.FullName)
                     .Append(SEP)
                     .Append(fieldName);

            if (objectID.HasUsefulData())
                m_builder.Append(SEP).Append(objectID);

            if (m_keyExt != null)
                m_builder.Append(SEP).Append(m_keyExt);

            return m_builder.ToString();
        }
    }
}
