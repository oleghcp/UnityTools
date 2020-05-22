using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityUtility.Async;
using UnityUtility.MathExt;
using UnityUtility.SaveLoad.SaveProviderStuff;

namespace UnityUtility.SaveLoad
{
    /// <summary>
    /// Keeps, saves and loads game data.
    /// </summary>
    public sealed class SaveProvider
    {
        private const BindingFlags MASK = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private ISaver m_innerSaver;
        private IKeyGenerator m_keyGenerator;
        private Dictionary<object, List<SaveLoadFieldAttribute>> m_fields = new Dictionary<object, List<SaveLoadFieldAttribute>>();

        public ISaver Saver
        {
            get { return m_innerSaver; }
        }

        public SaveProvider(ISaver saver)
        {
            m_innerSaver = saver;
            m_keyGenerator = new BaseKeyGenerator();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="saver">Object which saves and loads objects data. Default saver is UnityEngine.PlayerPrefs wrapper.</param>
        /// <param name="keyGenerator">Object which generates keys for key-value storage.</param>
        /// 
        public SaveProvider(ISaver saver, IKeyGenerator keyGenerator)
        {
            m_innerSaver = saver;
            m_keyGenerator = keyGenerator;
        }

        ////////////////
        //Public funcs//
        ////////////////

        /// <summary>
        /// Registers an object of which fields should be saved and load.
        /// </summary>
        /// <param name="initFields">If true the registered object fields will be initialized from saved data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RegMember(object client, bool initFields = true)
        {
            RegMember(client, null, initFields);
        }

        /// <summary>
        /// Registers an object of which fields should be saved and load.
        /// </summary>
        /// <param name="clientId">Specific id for the client if there are more than one instance of the client class.</param>
        /// <param name="initFields">If true the registered object fields will be initialized from saved data.</param>
        public void RegMember(object client, string clientId, bool initFields = true)
        {
            Type t = client.GetType();
            FieldInfo[] fields = t.GetFields(MASK);

            List<SaveLoadFieldAttribute> list = null;

            for (int i = 0; i < fields.Length; i++)
            {
                SaveLoadFieldAttribute a = fields[i].GetCustomAttribute<SaveLoadFieldAttribute>();

                if (a != null)
                {
                    if (list == null)
                        list = new List<SaveLoadFieldAttribute>();

                    a.Field = fields[i];
                    a.Key = m_keyGenerator.Generate(t, a.Field.Name, clientId);

                    list.Add(a);

                    if (initFields)
                    {
                        object defaultValue = a.DefValue ?? a.Field.FieldType.GetDefaultValue();

                        object value = defaultValue != null ?
                                       m_innerSaver.Get(a.Key, defaultValue) :
                                       m_innerSaver.Get(a.Key, a.Field.FieldType);

                        a.Field.SetValue(client, value);
                    }
                }
            }

            if (list != null)
                m_fields.Add(client, list);
        }

        /// <summary>
        /// Unregisters the registered object.
        /// </summary>        
        /// <param name="deleteSaves">Removes key-value data of the object from the storage if true. You should call ApplyAll Function to save changes.</param>
        public void UnregMember(object client, bool deleteSaves = false)
        {
            if (deleteSaves && m_fields.TryGetValue(client, out var list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    m_innerSaver.DeleteKey(list[i].Key);
                }
            }

            m_fields.Remove(client);
        }

        /// <summary>
        /// Unregisters all registered objects.
        /// </summary>        
        /// <param name="deleteSaves">Removes key-value data of the objects from the storage if true. You should call ApplyAll Function to save changes.</param>
        public void UnregAllMembers(bool deleteSaves = false)
        {
            if (deleteSaves)
            {
                foreach (var kvp in m_fields)
                {
                    List<SaveLoadFieldAttribute> list = kvp.Value;

                    for (int i = 0; i < list.Count; i++)
                    {
                        m_innerSaver.DeleteKey(list[i].Key);
                    }
                }
            }

            m_fields.Clear();
        }

        /// <summary>
        /// Saves all registered data.
        /// </summary>
        public void Save(string version)
        {
            f_collect();
            m_innerSaver.SaveVersion(version);
        }

        /// <summary>
        /// Saves all registered data asynchronously.
        /// </summary>
        public TaskInfo SaveAsync(string version, int stepsPerFrame)
        {
            IEnumerator CollectAndSave()
            {
                stepsPerFrame = stepsPerFrame.CutBefore(1);

                int counter = 0;

                foreach (var kvp in m_fields)
                {
                    List<SaveLoadFieldAttribute> aList = kvp.Value;

                    for (int i = 0; i < aList.Count; i++)
                    {
                        m_innerSaver.Set(aList[i].Key, aList[i].Field.GetValue(kvp.Key));

                        if (++counter >= stepsPerFrame)
                        {
                            counter = 0;
                            yield return null;
                        }
                    }
                }

                TaskInfo saveTask = m_innerSaver.SaveVersionAsync(version, stepsPerFrame);

                while (saveTask.IsAlive)
                {
                    yield return null;
                }
            }

            return TaskSystem.StartAsync(CollectAndSave());
        }

        public void Load(string version)
        {
            m_innerSaver.LoadVersion(version);
        }

        /// <summary>
        /// Clears current storage.
        /// </summary>
        public void Clear()
        {
            m_innerSaver.Clear();
        }

        ///////////////
        //Inner funcs//
        ///////////////

        private void f_collect()
        {
            foreach (var kvp in m_fields)
            {
                List<SaveLoadFieldAttribute> aList = kvp.Value;
                for (int i = 0; i < aList.Count; i++)
                {
                    m_innerSaver.Set(aList[i].Key, aList[i].Field.GetValue(kvp.Key));
                }
            }
        }
    }
}
