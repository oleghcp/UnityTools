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

        private Saver m_innerSaver;
        private Serializer m_serializer;
        private BinarySerializer m_binary;
        private KeyGenerator m_keyGenerator;
        private Dictionary<object, List<SaveLoadFieldAttribute>> m_fields = new Dictionary<object, List<SaveLoadFieldAttribute>>();

        public Saver Saver
        {
            get { return m_innerSaver; }
        }

        public SaveProvider()
        {
            m_innerSaver = new PlayerPrefsSaver();
            m_serializer = new JsonSerializer();
            m_keyGenerator = new BaseKeyGenerator();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="saver">Object which saves and loads objects data. Default saver is UnityEngine.PlayerPrefs wrapper.</param>
        /// <param name="serializer">Object which is used for serializing custom type fields. Default serializer uses UnityEngine.JsonUtility.</param>
        /// <param name="keyGenerator">Object which generates keys for key-value storage.</param>
        public SaveProvider(Saver saver, Serializer serializer, KeyGenerator keyGenerator)
        {
            m_innerSaver = saver;
            m_serializer = serializer;
            m_keyGenerator = keyGenerator;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="saver">Object which saves and loads objects data. Default saver is UnityEngine.PlayerPrefs wrapper.</param>
        /// <param name="serializer">Object which is used for binary serializing custom type fields.</param>
        /// <param name="keyGenerator">Object which generates keys for key-value storage.</param>
        public SaveProvider(Saver saver, BinarySerializer serializer, KeyGenerator keyGenerator)
        {
            m_innerSaver = saver;
            m_binary = serializer;
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
                    if (list == null) { list = new List<SaveLoadFieldAttribute>(); }

                    a.Field = fields[i];
                    a.Key = m_keyGenerator.Generate(t, a.Field.Name, clientId);

                    list.Add(a);

                    if (initFields) { a.Field.SetValue(client, f_get(a)); }
                }
            }

            if (list != null) { m_fields.Add(client, list); }
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
                    m_innerSaver.Delete(list[i].Key);
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
                        m_innerSaver.Delete(list[i].Key);
                    }
                }
            }

            m_fields.Clear();
        }

        /// <summary>
        /// Collects data from marked by SaveLoadFieldAttribute fields of all registered objects and sets it as key-value data through the saver.
        /// </summary>
        public void Collect()
        {
            foreach (var kvp in m_fields)
            {
                List<SaveLoadFieldAttribute> aList = kvp.Value;
                for (int i = 0; i < aList.Count; i++)
                    f_set(aList[i].Key, aList[i].Field.GetValue(kvp.Key));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TaskInfo CollectAsync(int fieldPerFrame = 1)
        {
            return TaskSystem.StartAsync(CollectRoutine(fieldPerFrame));
        }

        /// <summary>
        /// Saves the collected data (calls PlayerPrefs.Save() in case of saver based on UnityEngine.PlayerPrefs).
        /// </summary>
        public void ApplyAll()
        {
            m_innerSaver.ApplyAll();
        }

        /// <summary>
        /// Saves the collected data to the specified package.
        /// </summary>
        public void ApplyAll(string versionName)
        {
            m_innerSaver.ApplyAll(versionName);
        }

        /// <summary>
        /// Deletes the entire collected data from the runtime storage.
        /// </summary>
        public void DeleteAll()
        {
            m_innerSaver.DeleteAll();
        }

        /// <summary>
        /// Calls Collect and ApplyAll functions.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SaveAll()
        {
            Collect();
            ApplyAll();
        }

        public void SaveAll(string versionName)
        {
            Collect();
            ApplyAll(versionName);
        }

        ///////////////
        //Inner funcs//
        ///////////////

        private void f_set(string key, object value)
        {
            if (value is int)
            {
                m_innerSaver.Set(key, (int)value);
            }
            else if (value is float)
            {
                m_innerSaver.Set(key, (float)value);
            }
            else if (value is bool)
            {
                m_innerSaver.Set(key, (bool)value);
            }
            else if (value is string)
            {
                m_innerSaver.Set(key, (string)value);
            }
            else
            {
                if (m_binary != null)
                    m_innerSaver.Set(key, m_binary.Serialize(value));
                else
                    m_innerSaver.Set(key, m_serializer.Serialize(value));
            }
        }

        private object f_get(SaveLoadFieldAttribute a)
        {
            Type type = a.Field.FieldType;

            if (type == typeof(int))
            {
                return m_innerSaver.Get(a.Key, (int)a.DefValSimple);
            }
            else if (type == typeof(float))
            {
                return m_innerSaver.Get(a.Key, (float)a.DefValSimple);
            }
            else if (type == typeof(bool))
            {
                return m_innerSaver.Get(a.Key, (bool)a.DefValSimple);
            }
            else if (type == typeof(string))
            {
                return m_innerSaver.Get(a.Key, a.DefValString);
            }
            else
            {
                if (m_binary != null)
                {
                    byte[] bytes = m_innerSaver.Get(a.Key, (byte[])null);

                    if (bytes != null && bytes.Length > 0)
                        return m_binary.Deserialize(bytes, type);
                }
                else
                {
                    string serStr = m_innerSaver.Get(a.Key, (string)null);

                    if (serStr.HasAnyData())
                        return m_serializer.Deserialize(serStr, type);
                }

                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }
        }

        ////////////
        //Routines//
        ////////////

        IEnumerator CollectRoutine(int fieldPerFrame)
        {
            fieldPerFrame = fieldPerFrame.CutBefore(1);

            int counter = 0;

            foreach (var kvp in m_fields)
            {
                List<SaveLoadFieldAttribute> aList = kvp.Value;

                for (int i = 0; i < aList.Count; i++)
                {
                    f_set(aList[i].Key, aList[i].Field.GetValue(kvp.Key));

                    if (++counter >= fieldPerFrame)
                    {
                        counter = 0;
                        yield return null;
                    }
                }
            }
        }
    }
}
