using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityUtility.Async;
using UnityUtility.MathExt;

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
            f_recreateVersion(version);

            foreach (var kvp in m_storage)
            {
                File.WriteAllText(kvp.Key, kvp.Value);
            }
        }

        public TaskInfo SaveVersionAsync(string version, int keysPerFrame)
        {
            f_createProfile();
            f_recreateVersion(version);

            keysPerFrame = keysPerFrame.CutBefore(1);

            IEnumerator wrilteFile()
            {
                int count = 0;

                foreach (var kvp in m_storage)
                {
                    File.WriteAllText(kvp.Key, kvp.Value);

                    if (++count % keysPerFrame == 0)
                        yield return null;
                }
            }

            return TaskSystem.StartAsync(wrilteFile());
        }

        public void LoadVersion(string version)
        {
            f_createProfile();
            m_storage.Clear();

            string versionParh = f_getVersionPath(version);
            if (Directory.Exists(versionParh))
            {
                foreach (var filePath in Directory.EnumerateFiles(versionParh))
                {
                    m_storage[PathUtility.GetName(filePath)] = File.ReadAllText(filePath);
                }
            }
        }

        public void DeleteVersion(string version)
        {
            string versionParh = f_getVersionPath(version);
            if (Directory.Exists(versionParh))
                Directory.Delete(versionParh, true);
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

        private void f_createProfile()
        {
            Directory.CreateDirectory(PROFILE_PATH);
        }

        private T f_get<T>(string key, T defVal, Func<string, T> parse)
        {
            m_storage.TryGetValue(key, out string data);
            return data.HasUsefulData() ? parse(data) : defVal;
        }

        private string f_getVersionPath(string version)
        {
            return Path.Combine(PROFILE_PATH, version);
        }

        private void f_recreateVersion(string version)
        {
            string versionPath = f_getVersionPath(version);

            if (Directory.Exists(versionPath))
                Directory.Delete(versionPath, true);

            Directory.CreateDirectory(version);
        }
    }
}
