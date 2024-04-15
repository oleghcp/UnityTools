using System;
using System.Collections.Generic;
using OlegHcp.Async;

namespace OlegHcp.SaveLoad
{
    public interface ISaver
    {
        bool LoadVersion(string version);
        bool DeleteVersion(string version);
        void SaveVersion(string version);
        TaskInfo SaveVersionAsync(string version);
        string[] GetVersionList();
        void GetVersionList(List<string> versions);

        void DeleteKey(string key);
        void Set(string key, object value);
        object Get(string key, object defaultValue);
        object Get(string key, Type type);
    }

    public interface IKeyGenerator
    {
        string Generate(Type objectType, string fieldName, string objectID = null);
    }
}
