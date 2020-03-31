using System;
using UnityEngine;

namespace UnityUtility.SaveLoad.SaveProviderStuff
{
    /// <summary>
    /// A serializer for SaveProvider (UnityEngine.JsonUtility wrapper).
    /// </summary>
    public sealed class JsonSerializer : Serializer
    {
        public string Serialize(object toSerialize)
        {
            return JsonUtility.ToJson(toSerialize);
        }

        public object Deserialize(string toDeserialize, Type type)
        {
            return JsonUtility.FromJson(toDeserialize, type);
        }
    }
}
