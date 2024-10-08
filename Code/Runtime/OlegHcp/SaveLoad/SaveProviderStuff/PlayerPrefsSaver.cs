﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OlegHcp.Async;
using OlegHcp.CSharp;
using OlegHcp.Mathematics;
using UnityEngine;

namespace OlegHcp.SaveLoad.SaveProviderStuff
{
    public class PlayerPrefsSaver : ISaver
    {
        bool ISaver.DeleteVersion(string version)
        {
            PlayerPrefs.DeleteAll();
            return true;
        }

        bool ISaver.LoadVersion(string version)
        {
            throw new NotImplementedException();
        }

        void ISaver.SaveVersion(string version)
        {
            PlayerPrefs.Save();
        }

        TaskInfo ISaver.SaveVersionAsync(string version)
        {
            PlayerPrefs.Save();
            return default;
        }

        string[] ISaver.GetVersionList()
        {
            throw new NotImplementedException();
        }

        void ISaver.GetVersionList(List<string> versions)
        {
            throw new NotImplementedException();
        }

        void ISaver.DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        void ISaver.SaveValue(string key, object value)
        {
            TypeCode typeCode = value.GetType().GetTypeCode();

            switch (typeCode)
            {
                case TypeCode.String:
                    PlayerPrefs.SetString(key, (string)value);
                    break;

                case TypeCode.Int32:
                    PlayerPrefs.SetInt(key, (int)value);
                    break;

                case TypeCode.Single:
                    PlayerPrefs.SetFloat(key, (float)value);
                    break;

                case TypeCode.Boolean:
                    PlayerPrefs.SetInt(key, ((bool)value).ToInt());
                    break;

                case TypeCode.Object:
                    PlayerPrefs.SetString(key, JsonUtility.ToJson(value));
                    break;

                default:
                    throw new SwitchExpressionException(typeCode);
            }
        }

        bool ISaver.TryLoadValue(string key, Type type, out object value)
        {
            if (PlayerPrefs.HasKey(key))
            {
                value = GetInternal(key, type);
                return true;
            }

            value = null;
            return false;
        }

        private object GetInternal(string key, Type type)
        {
            switch (type.GetTypeCode())
            {
                case TypeCode.String: return PlayerPrefs.GetString(key);
                case TypeCode.Int32: return PlayerPrefs.GetInt(key);
                case TypeCode.Single: return PlayerPrefs.GetFloat(key);
                case TypeCode.Boolean: return PlayerPrefs.GetInt(key) > 0;
                case TypeCode.Object: return JsonUtility.FromJson(PlayerPrefs.GetString(key), type);
                default: throw new SwitchExpressionException(type.GetTypeCode());
            }
        }
    }
}
