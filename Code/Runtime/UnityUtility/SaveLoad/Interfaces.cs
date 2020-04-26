using System;

namespace UnityUtility.SaveLoad
{
    public interface ISaver
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

        void Delete(string key);
        bool HasKey(string key);

        void ApplyAll();
        void ApplyAll(string versionName);
        void DeleteAll();
    }

    public interface ISerializer
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
