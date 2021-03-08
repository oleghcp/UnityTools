using System;
using System.Text;

namespace UnityUtility.SaveLoad.SaveProviderStuff
{
    public class BaseKeyGenerator : IKeyGenerator
    {
        private const char SEP = '.';

        private StringBuilder _builder;

        public BaseKeyGenerator()
        {
            _builder = new StringBuilder();
        }

        public string Generate(Type objectType, string fieldName, string objectID)
        {
            _builder.Clear()
                     .Append(objectType.FullName)
                     .Append(SEP)
                     .Append(fieldName);

            if (objectID.HasUsefulData())
                _builder.Append(SEP).Append(objectID);

            return _builder.ToString();
        }
    }
}
