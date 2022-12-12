using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityUtility.Async;
using UnityUtility.CSharp.Collections;
using UnityUtility.Mathematics;
using UnityUtility.SaveLoad.SaveProviderStuff;
using UnityUtility.Tools;

namespace UnityUtility.SaveLoad
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
        private Dictionary<object, List<SaveLoadFieldAttribute>> _fields = new Dictionary<object, List<SaveLoadFieldAttribute>>();

        public SaveProvider(ISaver saver)
        {
            if (saver == null)
                throw Errors.NullParameter(nameof(saver));

            _saver = saver;
            _keyGenerator = new BaseKeyGenerator();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="saver">Object which saves and loads objects data. Default saver is UnityEngine.PlayerPrefs wrapper.</param>
        /// <param name="keyGenerator">Object which generates keys for key-value storage.</param>
        /// 
        public SaveProvider(ISaver saver, IKeyGenerator keyGenerator)
        {
            if (saver == null)
                throw Errors.NullParameter(nameof(saver));

            if (keyGenerator == null)
                throw Errors.NullParameter(nameof(keyGenerator));

            _saver = saver;
            _keyGenerator = keyGenerator;
        }

        public void ReplaceSaver(ISaver saver)
        {
            if (saver == null)
                throw Errors.NullParameter(nameof(saver));

            _saver = saver;
        }

        /// <summary>
        /// Registers an object of which fields should be saved and load.
        /// </summary>
        /// <param name="initFields">If true the registered object fields will be initialized from saved data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RegMember(object fieldsOwner, bool initFields = true)
        {
            RegMember(fieldsOwner, null, initFields);
        }

        /// <summary>
        /// Registers an object of which fields should be saved and load.
        /// </summary>
        /// <param name="ownerId">Specific id for the fields owner if there are more than one instance of the owner class.</param>
        /// <param name="initFields">If true the registered object fields will be initialized from saved data.</param>
        public void RegMember(object fieldsOwner, string ownerId, bool initFields = true)
        {
            if (fieldsOwner == null)
                throw Errors.NullParameter(nameof(fieldsOwner));

            Type t = fieldsOwner.GetType();
            FieldInfo[] fields = t.GetFields(FIELD_MASK);

            List<SaveLoadFieldAttribute> list = null;

            for (int i = 0; i < fields.Length; i++)
            {
                SaveLoadFieldAttribute a = fields[i].GetCustomAttribute<SaveLoadFieldAttribute>();

                if (a != null)
                {
                    if (list == null)
                        list = new List<SaveLoadFieldAttribute>();

                    a.Field = fields[i];
                    a.Key = _keyGenerator.Generate(t, a.Field.Name, ownerId);

                    list.Add(a);

                    if (initFields)
                    {
                        object value = a.DefValue != null ? _saver.Get(a.Key, a.DefValue)
                                                          : _saver.Get(a.Key, a.Field.FieldType);
                        a.Field.SetValue(fieldsOwner, value);
                    }
                }
            }

            if (list != null)
                _fields.Add(fieldsOwner, list);
        }

        /// <summary>
        /// Unregisters the registered object.
        /// </summary>        
        /// <param name="deleteSaves">Removes key-value data of the object from the storage if true. You should call ApplyAll Function to save changes.</param>
        public void UnregMember(object fieldsOwner, UnregOption option = UnregOption.None)
        {
            if (fieldsOwner == null)
                throw Errors.NullParameter(nameof(fieldsOwner));

            if (!_fields.Remove(fieldsOwner, out var aList) || option == UnregOption.None)
                return;

            foreach (var attribute in aList)
            {
                if (option == UnregOption.SaveObjectState)
                    _saver.Set(attribute.Key, attribute.Field.GetValue(fieldsOwner));
                else if (option == UnregOption.DeleteObjectState)
                    _saver.DeleteKey(attribute.Key);
            }
        }

        /// <summary>
        /// Unregisters all registered objects.
        /// </summary>        
        /// <param name="deleteSaves">Removes key-value data of the objects from the storage if true. You should call ApplyAll Function to save changes.</param>
        public void UnregAllMembers(UnregOption option = UnregOption.None)
        {
            if (option == UnregOption.DeleteObjectState)
            {
                foreach (var aList in _fields.Values)
                {
                    foreach (var attribute in aList)
                        _saver.DeleteKey(attribute.Key);
                }
            }
            else if (option == UnregOption.SaveObjectState)
            {
                foreach (var (fieldsOwner, aList) in _fields)
                {
                    foreach (var attribute in aList)
                        _saver.Set(attribute.Key, attribute.Field.GetValue(fieldsOwner));
                }
            }

            _fields.Clear();
        }

        public bool LoadVersion(string version)
        {
            return _saver.LoadVersion(version);
        }

        public bool DeleteVersion(string version)
        {
            return _saver.DeleteVersion(version);
        }

        /// <summary>
        /// Saves all registered data.
        /// </summary>
        public void Save(string version)
        {
            Collect();
            _saver.SaveVersion(version);
        }

        /// <summary>
        /// Saves all registered data.
        /// </summary>
        public void Save()
        {
            Collect();
            _saver.SaveLastVersion();
        }

        /// <summary>
        /// Saves all registered data asynchronously.
        /// </summary>
        public TaskInfo SaveAsync(string version, int stepsPerFrame)
        {
            return getRoutine(stepsPerFrame.ClampMin(1)).StartAsync();

            IEnumerator getRoutine(int spf)
            {
                yield return GetCollectRountine(spf).StartAsync();
                yield return _saver.SaveVersionAsync(version, spf);
            }
        }

        /// <summary>
        /// Saves all registered data asynchronously.
        /// </summary>
        public TaskInfo SaveAsync(int stepsPerFrame)
        {
            return getRoutine(stepsPerFrame.ClampMin(1)).StartAsync();

            IEnumerator getRoutine(int spf)
            {
                yield return GetCollectRountine(spf).StartAsync();
                yield return _saver.SaveLastVersionAsync(spf);
            }
        }

        /// <summary>
        /// Clears current storage.
        /// </summary>
        public void Clear()
        {
            _saver.Clear();
        }

        private void Collect()
        {
            foreach (var (fieldsOwner, aList) in _fields)
            {
                for (int i = 0; i < aList.Count; i++)
                {
                    _saver.Set(aList[i].Key, aList[i].Field.GetValue(fieldsOwner));
                }
            }
        }

        private IEnumerator GetCollectRountine(int stepsPerFrame)
        {
            int counter = 0;

            foreach (var (fieldsOwner, aList) in _fields)
            {
                for (int i = 0; i < aList.Count; i++)
                {
                    _saver.Set(aList[i].Key, aList[i].Field.GetValue(fieldsOwner));

                    if (++counter >= stepsPerFrame)
                    {
                        counter = 0;
                        yield return null;
                    }
                }
            }
        }
    }
}
