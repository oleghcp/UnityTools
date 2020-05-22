using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.Async;

namespace UnityUtility.SaveLoad.SaveProviderStuff
{
    /// <summary>
    /// Saves all data to non-encrypted text files. Can be used for debuging and so on.
    /// </summary>
    public sealed class DebugModeSaver : ISaver
    {
        private readonly string PROFILE_PATH;

        private Dictionary<string, string> m_storage = new Dictionary<string, string>();

        public DebugModeSaver(string profileName)
        {
            PROFILE_PATH = $"SaveGame/{profileName}/";
            f_createProfile();
        }

        public void SaveVersion(string version)
        {
            f_createProfile();
            string versionPath = PROFILE_PATH + version;

            ArrayWrapper wrapper = new ArrayWrapper
            {
                Array = m_storage.Select(Container.Create).ToArray()
            };

            File.WriteAllText(versionPath, JsonUtility.ToJson(wrapper, true));
        }

        public TaskInfo SaveVersionAsync(string version, int keysPerFrame)
        {
            throw new NotImplementedException();
        }

        public void LoadVersion(string version)
        {
            f_createProfile();

            string versionPath = PROFILE_PATH + version;

            if (!File.Exists(versionPath))
            {
                m_storage.Clear();
                return;
            }

            string json = File.ReadAllText(versionPath);
            ArrayWrapper wrapper = JsonUtility.FromJson<ArrayWrapper>(json);

            m_storage = wrapper.Array.ToDictionary(item => item.Key, item => item.Value);
        }

        public void DeleteVersion(string version)
        {
            string versionPath = PROFILE_PATH + version;
            if (File.Exists(versionPath))
                File.Delete(versionPath);
        }

        public void DeleteProfile()
        {
            m_storage.Clear();
            if (Directory.Exists(PROFILE_PATH))
                Directory.Delete(PROFILE_PATH, true);
        }

        //--//

        public bool Get(string key, bool defVal)
        {
            return f_get(key, defVal, bool.Parse);
        }

        public float Get(string key, float defVal)
        {
            return f_get(key, defVal, float.Parse);
        }

        public int Get(string key, int defVal)
        {
            return f_get(key, defVal, int.Parse);
        }

        public string Get(string key, string defVal)
        {
            return f_get(key, defVal, value => value);
        }

        public byte[] Get(string key, byte[] defVal)
        {
            return f_get(key, defVal, ByteArrayUtility.FromString);
        }

        //--//

        public void Set(string key, bool value)
        {
            m_storage[key] = value.ToString();
        }

        public void Set(string key, string value)
        {
            m_storage[key] = value;
        }

        public void Set(string key, float value)
        {
            m_storage[key] = value.ToString();
        }

        public void Set(string key, int value)
        {
            m_storage[key] = value.ToString();
        }

        public void Set(string key, byte[] value)
        {
            m_storage[key] = ByteArrayUtility.ToString(value);
        }

        // -- //

        public bool HasKey(string key)
        {
            return m_storage.ContainsKey(key);
        }

        public void DeleteKey(string key)
        {
            m_storage.Remove(key);
        }

        public void Clear()
        {
            m_storage.Clear();
        }

        //--//

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void f_createProfile()
        {
            Directory.CreateDirectory(PROFILE_PATH);
        }

        private T f_get<T>(string key, T defVal, Func<string, T> parse)
        {
            m_storage.TryGetValue(key, out string data);
            return data.HasUsefulData() ? parse(data) : defVal;
        }

        //--//

        [Serializable]
        private struct Container
        {
            public string Key;
            public string Value;

            public static Container Create(KeyValuePair<string, string> kvp)
            {
                return new Container { Key = kvp.Key, Value = kvp.Value };
            }
        }

        [Serializable]
        private struct ArrayWrapper
        {
            public Container[] Array;
        }
    }
}
