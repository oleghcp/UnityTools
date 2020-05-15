using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.Async;
using UnityUtility.MathExt;

namespace UnityUtility.SaveLoad.SaveProviderStuff
{
    /// <summary>
    /// Saves and loads data through UnityEngine.PlayerPrefs.
    /// </summary>
    public sealed class PlayerPrefsSaver : ISaver
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            PlayerPrefs.DeleteAll();
        }

        //--//

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Get(string key, int defVal)
        {
            return PlayerPrefs.GetInt(key, defVal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Get(string key, float defVal)
        {
            return PlayerPrefs.GetFloat(key, defVal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Get(string key, bool defVal)
        {
            return PlayerPrefs.GetInt(key, defVal.ToInt()).ToBool();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Get(string key, string defVal)
        {
            return PlayerPrefs.GetString(key, defVal);
        }

        public byte[] Get(string key, byte[] defVal)
        {
            string data = PlayerPrefs.GetString(key);
            return data.HasUsefulData() ? ByteArrayUtility.FromString(data) : defVal;
        }

        //--//

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value.ToInt());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public void Set(string key, byte[] value)
        {
            PlayerPrefs.SetString(key, ByteArrayUtility.ToString(value));
        }

        // -- //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SaveVersion(string _)
        {
            PlayerPrefs.Save();
        }

        public TaskInfo SaveVersionAsync(string version, int keysPerFrame)
        {
            throw new NotImplementedException();
        }

        public void LoadVersion(string version)
        {
            throw new NotImplementedException();
        }

        public void DeleteVersion(string version)
        {
            throw new NotImplementedException();
        }

        public void DeleteProfile()
        {
            throw new NotImplementedException();
        }
    }
}
