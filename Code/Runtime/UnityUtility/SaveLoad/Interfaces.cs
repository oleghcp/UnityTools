using System;
using UnityUtility.Async;

namespace UnityUtility.SaveLoad
{
    public interface ISaver
    {
        bool StorageLoaded { get; }
        bool ProfiledCreated { get; }

        void Set(string key, object value);
        object Get(string key, object defaltValue);
        object Get(string key, Type type);

        bool HasKey(string key);
        void DeleteKey(string key);
        void Clear();

        bool CreateProfile();
        bool DeleteProfile();
        void SaveVersion(string version);
        TaskInfo SaveVersionAsync(string version, int keysPerFrame);
        bool LoadVersion(string version);
        bool DeleteVersion(string version);
    }

    public interface IKeyGenerator
    {
        string Generate(Type objectType, string fieldName, string objectID);
    }
}
