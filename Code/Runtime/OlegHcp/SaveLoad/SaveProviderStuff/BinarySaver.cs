using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OlegHcp.Async;
using OlegHcp.CSharp;
using OlegHcp.IO;
using OlegHcp.Tools;
using UnityEngine;

namespace OlegHcp.SaveLoad.SaveProviderStuff
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
                try { _storage = BinaryFileUtility.Load<Dictionary<string, object>>(path); }
                catch (Exception) { return false; }
                return true;
            }

            return false;
        }

        void ISaver.SaveVersion(string version)
        {
            string path = Path.Combine(_dataPath, version);
            BinaryFileUtility.Save(path, _storage);
        }

        TaskInfo ISaver.SaveVersionAsync(string version)
        {
            return getRoutine().StartAsync();

            IEnumerator getRoutine()
            {
                Task<string> task = Task.Run(asyncSave);
                yield return new WaitForTask(task);

                if (task.Result != null)
                    Debug.LogError(task.Result);
            }

            string asyncSave()
            {
                string path = Path.Combine(_dataPath, version);

                try { BinaryFileUtility.Save(path, _storage); }
                catch (Exception e) { return e.Message; }

                return null;
            }
        }

        string[] ISaver.GetVersionList()
        {
            return Directory.EnumerateFiles(_dataPath)
                            .Select(Path.GetFileName)
                            .ToArray();
        }

        void ISaver.GetVersionList(List<string> versions)
        {
            IEnumerable<string> collection = Directory.EnumerateFiles(_dataPath)
                                                      .Select(Path.GetFileName);
            versions.Clear();
            versions.AddRange(collection);
        }

        void ISaver.DeleteKey(string key)
        {
            _storage.Remove(key);
        }

        void ISaver.SaveValue(string key, object value)
        {
            _storage[key] = value;
        }

        object ISaver.Get(string key, object defaultValue)
        {
            if (_storage.TryGetValue(key, out object value))
                return value;

            return defaultValue;
        }

        object ISaver.Get(string key, Type type)
        {
            if (_storage.TryGetValue(key, out object value))
                return value;

            return type.GetDefaultValue();
        }
    }
}
