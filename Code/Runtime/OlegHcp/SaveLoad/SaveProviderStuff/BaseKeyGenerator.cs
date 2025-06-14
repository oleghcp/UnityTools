using System;
using System.Reflection;
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

        public string Generate(Type objectType, FieldInfo field, string objectID, string keyDefinedByUser)
        {
            const char separator = '.';

            if (keyDefinedByUser.IsNullOrWhiteSpace())
            {
                _builder.Append(GetName())
                        .Append(separator)
                        .Append(field.Name);
            }
            else
            {
                _builder.Append(keyDefinedByUser);
            }

            if (objectID.HasUsefulData())
                _builder.Append(separator)
                        .Append(objectID);

            return _builder.Cut();

            string GetName()
            {
                return _considerNamespace ? objectType.FullName
                                          : objectType.Name;
            }
        }
    }
}
