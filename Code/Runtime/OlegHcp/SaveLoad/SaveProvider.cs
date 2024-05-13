using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using OlegHcp.Async;
#if !UNITY_2021_2_OR_NEWER
using OlegHcp.CSharp.Collections;
#endif
using OlegHcp.Mathematics;
using OlegHcp.SaveLoad.SaveProviderStuff;
using OlegHcp.Tools;

namespace OlegHcp.SaveLoad
{
    public enum UnregOption
    {
        None,
        SaveObjectState,
        DeleteObjectState,
    }

    /// <summary>
    /// Keeps, saves and loads game data.
    /// </summary>
    public sealed class SaveProvider
    {
        private const BindingFlags FIELD_MASK = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private ISaver _saver;
        private IKeyGenerator _keyGenerator;
        private Dictionary<object, List<SaveLoadFieldAttribute>> _storage = new Dictionary<object, List<SaveLoadFieldAttribute>>();
        private int _asyncSaveFieldsPerFrame = 50;

        public int AsyncSaveFieldsPerFrame
        {
            get => _asyncSaveFieldsPerFrame;
            set => _asyncSaveFieldsPerFrame = value.ClampMin(1);
        }

        public SaveProvider()
        {
            _saver = new PlayerPrefsSaver();
            _keyGenerator = new BaseKeyGenerator();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="saver">Object which saves and loads objects data. Default saver is UnityEngine.PlayerPrefs wrapper.</param>
        public SaveProvider(ISaver saver)
        {
            if (saver == null)
                throw ThrowErrors.NullParameter(nameof(saver));

            _saver = saver;
            _keyGenerator = new BaseKeyGenerator();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="saver">Object which saves and loads objects data. Default saver is UnityEngine.PlayerPrefs wrapper.</param>
        /// <param name="keyGenerator">Object which generates keys for key-value storage.</param>
        public SaveProvider(ISaver saver, IKeyGenerator keyGenerator)
        {
            if (saver == null)
                throw ThrowErrors.NullParameter(nameof(saver));

            if (keyGenerator == null)
                throw ThrowErrors.NullParameter(nameof(keyGenerator));

            _saver = saver;
            _keyGenerator = keyGenerator;
        }

        public void ReplaceSaver(ISaver saver)
        {
            if (saver == null)
                throw ThrowErrors.NullParameter(nameof(saver));

            _saver = saver;
        }

        /// <summary>
        /// Registers an object of which fields should be saved and loaded.
        /// </summary>
        /// <param name="initFields">If true the registered object fields will be initialized from saved data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RegMember(object fieldsOwner, bool initFields = true)
        {
            RegMember(fieldsOwner, null, initFields);
        }

        /// <summary>
        /// Registers an object of which fields should be saved and loaded.
        /// </summary>
        /// <param name="ownerId">Specific id for the fields owner if there are more than one instance of the owner class.</param>
        /// <param name="initFields">If true the registered object fields will be initialized from saved data.</param>
        public void RegMember(object fieldsOwner, string ownerId, bool initFields = true)
        {
            if (fieldsOwner == null)
                throw ThrowErrors.NullParameter(nameof(fieldsOwner));

            Type type = fieldsOwner.GetType();
            FieldInfo[] fields = type.GetFields(FIELD_MASK);

            List<SaveLoadFieldAttribute> list = null;

            for (int i = 0; i < fields.Length; i++)
            {
                SaveLoadFieldAttribute a = fields[i].GetCustomAttribute<SaveLoadFieldAttribute>();

                if (a == null)
                    continue;

                if (list == null)
                    list = new List<SaveLoadFieldAttribute>();

                a.Field = fields[i];
                a.Key = _keyGenerator.Generate(type, a.Key ?? a.Field.Name, ownerId);

                list.Add(a);

                if (initFields)
                    InitField(fieldsOwner, a);
            }

            if (list != null)
                _storage.Add(fieldsOwner, list);
        }

        /// <summary>
        /// Unregisters the registered object.
        /// </summary>        
        /// <param name="deleteSaves">Removes key-value data of the object from the storage if true. You should call ApplyAll Function to save changes.</param>
        public void UnregMember(object fieldsOwner, UnregOption option = UnregOption.None)
        {
            if (fieldsOwner == null)
                throw ThrowErrors.NullParameter(nameof(fieldsOwner));

            if (!_storage.Remove(fieldsOwner, out var aList))
                return;

            switch (option)
            {
                case UnregOption.SaveObjectState:
                    foreach (var attribute in aList)
                        _saver.SaveValue(attribute.Key, attribute.Field.GetValue(fieldsOwner));
                    break;

                case UnregOption.DeleteObjectState:
                    foreach (var attribute in aList)
                        _saver.DeleteKey(attribute.Key);
                    break;
            }
        }

        /// <summary>
        /// Unregisters all registered objects.
        /// </summary>        
        /// <param name="deleteSaves">Removes key-value data of the objects from the storage if true. You should call ApplyAll Function to save changes.</param>
        public void UnregAllMembers(UnregOption option = UnregOption.None)
        {
            switch (option)
            {
                case UnregOption.SaveObjectState:
                    foreach (var aList in _storage.Values)
                    {
                        foreach (var attribute in aList)
                            _saver.DeleteKey(attribute.Key);
                    }
                    break;

                case UnregOption.DeleteObjectState:
                    foreach (var (fieldsOwner, aList) in _storage)
                    {
                        foreach (var attribute in aList)
                            _saver.SaveValue(attribute.Key, attribute.Field.GetValue(fieldsOwner));
                    }
                    break;
            }

            _storage.Clear();
        }

        public bool Load(string version, bool initializeFields = false)
        {
            bool loaded = _saver.LoadVersion(version);

            if (loaded && initializeFields)
            {
                foreach (var (fieldsOwner, aList) in _storage)
                {
                    for (int i = 0; i < aList.Count; i++)
                    {
                        InitField(fieldsOwner, aList[i]);
                    }
                }
            }

            return loaded;
        }

        /// <summary>
        /// Delete saved data.
        /// </summary>
        public bool Delete(string version)
        {
            return _saver.DeleteVersion(version);
        }

        /// <summary>
        /// Saves all registered data.
        /// </summary>
        public void Save(string version, bool collectFields = true)
        {
            if (collectFields)
            {
                foreach (var (fieldsOwner, aList) in _storage)
                {
                    for (int i = 0; i < aList.Count; i++)
                    {
                        _saver.SaveValue(aList[i].Key, aList[i].Field.GetValue(fieldsOwner));
                    }
                }
            }

            _saver.SaveVersion(version);
        }

        /// <summary>
        /// Saves all registered data asynchronously.
        /// </summary>
        public TaskInfo SaveAsync(string version, bool collectFields = true)
        {
            if (collectFields)
                return getRoutine(_asyncSaveFieldsPerFrame).StartAsync();

            return _saver.SaveVersionAsync(version);

            IEnumerator getRoutine(int spf)
            {
                int counter = 0;

                yield return null;

                foreach (var (fieldsOwner, aList) in _storage)
                {
                    for (int i = 0; i < aList.Count; i++)
                    {
                        _saver.SaveValue(aList[i].Key, aList[i].Field.GetValue(fieldsOwner));

                        if (++counter >= spf)
                        {
                            counter = 0;
                            yield return null;
                        }
                    }
                }

                yield return _saver.SaveVersionAsync(version);
            }
        }

        public string[] GetVersionList()
        {
            return _saver.GetVersionList();
        }

        public void GetVersionList(List<string> versions)
        {
            _saver.GetVersionList(versions);
        }

        private void InitField(object fieldsOwner, SaveLoadFieldAttribute a)
        {
            object value = a.DefaultValue != null ? _saver.Get(a.Key, a.DefaultValue)
                                                  : _saver.Get(a.Key, a.Field.FieldType);
            a.Field.SetValue(fieldsOwner, value);
        }
    }
}
