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
        void SaveValue(string key, object value);
        bool TryLoadValue(string key, Type type, out object value);
    }

    public interface IKeyGenerator
    {
        string Generate(Type objectType, string fieldName, string objectID);
    }
}
