using System;
using System.Text;

namespace UnityUtility.SaveLoad.SaveProviderStuff
{
    public class BaseKeyGenerator : IKeyGenerator
    {
        private const char SEP = '.';

        private StringBuilder m_builder;

        public BaseKeyGenerator()
        {
            m_builder = new StringBuilder();
        }

        public string Generate(Type objectType, string fieldName, string objectID)
        {
            m_builder.Clear()
                     .Append(objectType.FullName)
                     .Append(SEP)
                     .Append(fieldName);

            if (objectID.HasUsefulData())
                m_builder.Append(SEP).Append(objectID);

            return m_builder.ToString();
        }
    }
}
