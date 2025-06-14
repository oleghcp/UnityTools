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
    public class SaveProvider
    {
        private const BindingFlags FIELD_MASK = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private readonly ISaver _saver;
        private readonly IKeyGenerator _keyGenerator;
        private readonly Dictionary<object, List<Container>> _storage = new Dictionary<object, List<Container>>();
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
        /// <param name="initFields">The fields of the registered object will be initialized from saved data if true.</param>
        /// <param name="declaredFieldsOnly">Save provider will ignore inherited fields if true.</param>
        public void RegMember(object fieldsOwner, string ownerId, bool initFields = true, bool declaredFieldsOnly = true)
        {
            if (fieldsOwner == null)
                throw ThrowErrors.NullParameter(nameof(fieldsOwner));

            List<Container> list = null;
            Type type = fieldsOwner.GetType();

            do
            {
                AnalyzeAndCollectFields(fieldsOwner, ownerId, type, initFields, ref list);
                type = type.BaseType;
            } while (!declaredFieldsOnly && type != typeof(object));

            if (list != null)
                _storage.Add(fieldsOwner, list);
        }

        private void AnalyzeAndCollectFields(object fieldsOwner, string ownerId, Type type, bool initFields, ref List<Container> list)
        {
            foreach (FieldInfo field in type.GetFields(FIELD_MASK))
            {
                var attribute = field.GetCustomAttribute<SaveLoadFieldAttribute>();

                if (attribute == null)
                    continue;

                if (list == null)
                    list = new List<Container>();

                Container container = new Container(field, _keyGenerator.Generate(type, field, ownerId, attribute.Key));
                list.Add(container);

                if (initFields)
                    container.InitField(fieldsOwner, _saver);
            }
        }

        /// <summary>
        /// Unregisters the registered object.
        /// </summary>        
        /// <param name="deleteSaves">Removes key-value data of the object from the storage if true. You should call ApplyAll Function to save changes.</param>
        public void UnregMember(object fieldsOwner, UnregOption option = UnregOption.None)
        {
            if (fieldsOwner == null)
                throw ThrowErrors.NullParameter(nameof(fieldsOwner));

            if (!_storage.Remove(fieldsOwner, out var list))
                return;

            switch (option)
            {
                case UnregOption.SaveObjectState:
                    foreach (var item in list)
                        _saver.SaveValue(item.Key, item.GetValue(fieldsOwner));
                    break;

                case UnregOption.DeleteObjectState:
                    foreach (var item in list)
                        _saver.DeleteKey(item.Key);
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
                    foreach (var list in _storage.Values)
                    {
                        foreach (var item in list)
                            _saver.DeleteKey(item.Key);
                    }
                    break;

                case UnregOption.DeleteObjectState:
                    foreach (var (fieldsOwner, list) in _storage)
                    {
                        foreach (var item in list)
                            _saver.SaveValue(item.Key, item.GetValue(fieldsOwner));
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
                        aList[i].InitField(fieldsOwner, _saver);
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
                        _saver.SaveValue(aList[i].Key, aList[i].GetValue(fieldsOwner));
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
                return TaskSystem.StartAsync(getRoutine(_asyncSaveFieldsPerFrame), true);

            return _saver.SaveVersionAsync(version);

            IEnumerator getRoutine(int spf)
            {
                int counter = 0;

                yield return null;

                foreach (var (fieldsOwner, aList) in _storage)
                {
                    for (int i = 0; i < aList.Count; i++)
                    {
                        _saver.SaveValue(aList[i].Key, aList[i].GetValue(fieldsOwner));

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

        private struct Container
        {
            private FieldInfo _field;
            private string _key;

            public string Key => _key;

            public Container(FieldInfo field, string key)
            {
                _field = field;
                _key = key;
            }

            public object GetValue(object fieldsOwner)
            {
                return _field.GetValue(fieldsOwner);
            }

            public void InitField(object fieldsOwner, ISaver saver)
            {
                if (saver.TryLoadValue(_key, _field.FieldType, out var value))
                    _field.SetValue(fieldsOwner, value);
            }
        }
    }
}
