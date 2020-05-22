using System;
using UnityUtility.Async;

namespace UnityUtility.SaveLoad
{
    public interface ISaver
    {
        void Set(string key, object value);
        object Get(string key, object defaltValue);
        object Get(string key, Type type);

        bool HasKey(string key);
        void DeleteKey(string key);
        void Clear();

        void SaveVersion(string version);
        TaskInfo SaveVersionAsync(string version, int keysPerFrame);
        void LoadVersion(string version);
        void DeleteVersion(string version);
        void DeleteProfile();
    }

    public interface IKeyGenerator
    {
        string Generate(Type objectType, string fieldName, string objectID);
    }
}
