using System;
using UU.MathExt;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace UU.SaveLoad.SaveProviderStuff
{
    /// <summary>
    /// Saves and loads data through UnityEngine.PlayerPrefs.
    /// </summary>
    public sealed class PlayerPrefsSaver : Saver
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ApplyAll()
        {
            PlayerPrefs.Save();
        }

        public void ApplyAll(string versionName)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        //--//

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
