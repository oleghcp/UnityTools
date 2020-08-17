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
    public enum UnregOption
    {
        None,
        SaveObjecState,
        DeleteObjecState,
    }

    /// <summary>
    /// Keeps, saves and loads game data.
    /// </summary>
    public sealed class SaveProvider
    {
        private const BindingFlags FIELD_MASK = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private ISaver m_saver;
        private IKeyGenerator m_keyGenerator;
        private Dictionary<object, List<SaveLoadFieldAttribute>> m_fields = new Dictionary<object, List<SaveLoadFieldAttribute>>();

        public ISaver Saver
        {
            get { return m_saver; }
        }

        public SaveProvider(ISaver saver)
        {
            m_saver = saver;
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
            m_saver = saver;
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
                    a.Key = m_keyGenerator.Generate(t, a.Field.Name, clientId);

                    list.Add(a);

                    if (initFields)
                    {
                        object defaultValue = a.DefValue ?? a.Field.FieldType.GetDefaultValue();
                        object value = m_saver.Get(a.Key, defaultValue, a.Field.FieldType);

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
        public void UnregMember(object client, UnregOption option = UnregOption.None)
        {
            List<SaveLoadFieldAttribute> aList = m_fields.PullOut(client);

            if (option == UnregOption.None || aList == null)
                return;

            foreach (var attribute in aList)
            {
                if (option == UnregOption.SaveObjecState)
                    m_saver.Set(attribute.Key, attribute.Field.GetValue(client));
                else if (option == UnregOption.DeleteObjecState)
                    m_saver.DeleteKey(attribute.Key);
            }
        }

        /// <summary>
        /// Unregisters all registered objects.
        /// </summary>        
        /// <param name="deleteSaves">Removes key-value data of the objects from the storage if true. You should call ApplyAll Function to save changes.</param>
        public void UnregAllMembers(UnregOption option = UnregOption.None)
        {
            if (option == UnregOption.DeleteObjecState)
            {
                foreach (var kvp in m_fields)
                {
                    foreach (var attribute in kvp.Value)
                        m_saver.DeleteKey(attribute.Key);
                }
            }
            else if (option == UnregOption.SaveObjecState)
            {
                foreach (var kvp in m_fields)
                {
                    foreach (var attribute in kvp.Value)
                        m_saver.Set(attribute.Key, attribute.Field.GetValue(kvp.Key));
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
            m_saver.SaveVersion(version);
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
                        m_saver.Set(aList[i].Key, aList[i].Field.GetValue(kvp.Key));

                        if (++counter >= stepsPerFrame)
                        {
                            counter = 0;
                            yield return null;
                        }
                    }
                }

                TaskInfo saveTask = m_saver.SaveVersionAsync(version, stepsPerFrame);

                while (saveTask.IsAlive)
                {
                    yield return null;
                }
            }

            return TaskSystem.StartAsync(CollectAndSave());
        }

        public bool Load(string version)
        {
            return m_saver.LoadVersion(version);
        }

        /// <summary>
        /// Clears current storage.
        /// </summary>
        public void Clear()
        {
            m_saver.Clear();
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
                    m_saver.Set(aList[i].Key, aList[i].Field.GetValue(kvp.Key));
                }
            }
        }
    }
}
