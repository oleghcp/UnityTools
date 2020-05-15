using System;
using UnityUtility.Async;

namespace UnityUtility.SaveLoad
{
    public interface IStorage
    {
        int Get(string key, int defVal);
        float Get(string key, float defVal);
        bool Get(string key, bool defVal);
        string Get(string key, string defVal);
        byte[] Get(string key, byte[] defVal);

        void Set(string key, int value);
        void Set(string key, float value);
        void Set(string key, bool value);
        void Set(string key, string value);
        void Set(string key, byte[] value);

        bool HasKey(string key);
        void DeleteKey(string key);
        void Clear();
    }

    public interface ISaver : IStorage
    {
        void SaveVersion(string version);
        TaskInfo SaveVersionAsync(string version, int keysPerFrame);
        void LoadVersion(string version);
        void DeleteVersion(string version);
        void DeleteProfile();
    }

    public interface ITextSerializer
    {
        string Serialize(object toSerialize);
        object Deserialize(string toDeserialize, Type type);
    }

    public interface IBinarySerializer
    {
        byte[] Serialize(object toSerialize);
        object Deserialize(byte[] toDeserialize, Type type);
    }

    public interface IKeyGenerator
    {
        string Generate(Type objectType, string fieldName, string objectID);
    }
}
