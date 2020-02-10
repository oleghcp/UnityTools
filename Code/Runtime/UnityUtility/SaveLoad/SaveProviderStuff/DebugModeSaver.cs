using System;
using System.IO;

namespace UU.SaveLoad.SaveProviderStuff
{
    /// <summary>
    /// Saves all data to non-encrypted text files. Can be used for debuging and so on.
    /// </summary>
    public sealed class DebugModeSaver : Saver
    {
        private const string DIR_NAME = "Save/";

        public DebugModeSaver()
        {
            f_checkDir();
        }

        public void ApplyAll() { }

        public void ApplyAll(string versionName)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll()
        {
            if (!Directory.Exists(DIR_NAME)) { return; }

            var info = new DirectoryInfo(DIR_NAME);
            var files = info.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                files[i].Delete();
            }
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
            throw new NotImplementedException();
        }

        //--//

        public void Set(string key, bool value)
        {
            f_set(key, value.ToString());
        }

        public void Set(string key, string value)
        {
            f_set(key, value);
        }

        public void Set(string key, float value)
        {
            f_set(key, value.ToString());
        }

        public void Set(string key, int value)
        {
            f_set(key, value.ToString());
        }

        public void Set(string key, byte[] value)
        {
            throw new NotImplementedException();
        }

        // -- //

        public void Delete(string key)
        {
            if (File.Exists(key))
            {
                File.Delete(DIR_NAME + key);
            }
        }

        public bool HasKey(string key)
        {
            return f_hasKey(key);
        }

        //--//

        private void f_checkDir()
        {
            if (!Directory.Exists(DIR_NAME))
            {
                Directory.CreateDirectory(DIR_NAME);
            }
        }

        private bool f_hasKey(string key)
        {
            f_checkDir();
            return File.Exists(DIR_NAME + key);
        }

        private T f_get<T>(string key, T defVal, Func<string, T> parse)
        {
            string data = f_load(key);
            return data.HasAnyData() ? parse(data) : defVal;
        }

        private void f_set(string key, string data)
        {
            f_checkDir();

            File.WriteAllText(DIR_NAME + key, data);
        }

        private string f_load(string key)
        {
            if (f_hasKey(key))
            {
                return File.ReadAllText(DIR_NAME + key);
            }

            return string.Empty;
        }        
    }
}
