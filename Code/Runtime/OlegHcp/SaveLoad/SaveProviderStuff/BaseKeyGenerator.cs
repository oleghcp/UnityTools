using System;
using System.Text;
using OlegHcp.CSharp;
using OlegHcp.CSharp.Text;

namespace OlegHcp.SaveLoad.SaveProviderStuff
{
    public class BaseKeyGenerator : IKeyGenerator
    {
        private StringBuilder _builder = new StringBuilder();
        private bool _considerNamespace;

        public BaseKeyGenerator(bool considerNamespace = true)
        {
            _considerNamespace = considerNamespace;
        }

        public string Generate(Type objectType, string fieldName, string objectID)
        {
            const char separator = '.';

            string typeName = _considerNamespace ? objectType.FullName
                                                 : objectType.Name;
            _builder.Append(typeName)
                    .Append(separator)
                    .Append(fieldName);

            if (objectID.HasUsefulData())
                _builder.Append(separator)
                        .Append(objectID);

            return _builder.Cut();
        }
    }
}
