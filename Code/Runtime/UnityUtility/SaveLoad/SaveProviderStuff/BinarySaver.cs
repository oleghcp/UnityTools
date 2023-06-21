using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityUtility.Async;
using UnityUtility.CSharp;
using UnityUtility.IO;
using UnityUtility.Tools;

namespace UnityUtility.SaveLoad.SaveProviderStuff
{
    public class BinarySaver : ISaver
    {
        private Dictionary<string, object> _storage = new Dictionary<string, object>();
        private string _dataPath;

        public BinarySaver()
        {
            _dataPath = Application.persistentDataPath;
        }

        public BinarySaver(string dataFolderPath)
        {
            if (dataFolderPath == null)
                throw ThrowErrors.NullParameter(nameof(dataFolderPath));

            _dataPath = dataFolderPath;
        }

        bool ISaver.DeleteVersion(string version)
        {
            string path = Path.Combine(_dataPath, version);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }

        bool ISaver.LoadVersion(string version)
        {
            string path = Path.Combine(_dataPath, version);

            if (File.Exists(path))
            {
                _storage = BinaryFileUtility.Load<Dictionary<string, object>>(path);
                return true;
            }

            return false;
        }

        void ISaver.SaveVersion(string version)
        {
            string path = Path.Combine(_dataPath, version);
            BinaryFileUtility.Save(path, _storage);
        }

        //TODO: make it async
        TaskInfo ISaver.SaveVersionAsync(string version)
        {
            string path = Path.Combine(_dataPath, version);
            BinaryFileUtility.Save(path, _storage);
            return default;
        }

        void ISaver.DeleteKey(string key)
        {
            _storage.Remove(key);
        }

        void ISaver.Set(string key, object value)
        {
            _storage[key] = value;
        }

        object ISaver.Get(string key, object defaltValue)
        {
            if (_storage.TryGetValue(key, out object value))
                return value;

            return defaltValue;
        }

        object ISaver.Get(string key, Type type)
        {
            if (_storage.TryGetValue(key, out object value))
                return value;

            return type.GetDefaultValue();
        }
    }
}
