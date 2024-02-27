using System;
using System.Text;
using OlegHcp.CSharp;
using OlegHcp.CSharp.Text;

namespace OlegHcp.SaveLoad.SaveProviderStuff
{
    public class BaseKeyGenerator : IKeyGenerator
    {
        private StringBuilder _builder = new StringBuilder();

        public string Generate(Type objectType, string fieldName, string objectID = null)
        {
            const char separator = '.';

            _builder.Append(objectType.FullName)
                    .Append(separator)
                    .Append(fieldName);

            if (objectID.HasUsefulData())
                _builder.Append(separator)
                        .Append(objectID);

            return _builder.Cut();
        }
    }
}
