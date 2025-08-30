using System;
using System.Collections.Generic;
using System.Globalization;
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
                case TypeCode.SByte:
                    PlayerPrefs.SetInt(key, (sbyte)value);
                    break;

                case TypeCode.Int16:
                    PlayerPrefs.SetInt(key, (short)value);
                    break;

                case TypeCode.Int32:
                    PlayerPrefs.SetInt(key, (int)value);
                    break;

                case TypeCode.Int64:
                    PlayerPrefs.SetString(key, value.ToString());
                    break;

                case TypeCode.Byte:
                    PlayerPrefs.SetInt(key, (byte)value);
                    break;

                case TypeCode.UInt16:
                    PlayerPrefs.SetInt(key, (ushort)value);
                    break;

                case TypeCode.UInt32:
                    PlayerPrefs.SetInt(key, (int)(uint)value);
                    break;

                case TypeCode.UInt64:
                    PlayerPrefs.SetString(key, value.ToString());
                    break;

                case TypeCode.Single:
                    PlayerPrefs.SetFloat(key, (float)value);
                    break;

                case TypeCode.Double:
                    PlayerPrefs.SetString(key, ((double)value).ToString(CultureInfo.InvariantCulture));
                    break;

                case TypeCode.Decimal:
                    PlayerPrefs.SetString(key, ((decimal)value).ToString(CultureInfo.InvariantCulture));
                    break;

                case TypeCode.Char:
                    PlayerPrefs.SetInt(key, (char)value);
                    break;

                case TypeCode.Boolean:
                    PlayerPrefs.SetInt(key, ((bool)value).ToInt());
                    break;

                case TypeCode.String:
                    PlayerPrefs.SetString(key, (string)value);
                    break;

                case TypeCode.DateTime:
                    PlayerPrefs.SetString(key, ((DateTime)value).Ticks.ToString());
                    break;

                case TypeCode.Object:
                    PlayerPrefs.SetString(key, JsonUtility.ToJson(value));
                    break;

                default:
                    throw new UnsupportedValueException(typeCode);
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
            TypeCode typeCode = type.GetTypeCode();

            switch (typeCode)
            {
                case TypeCode.SByte:
                    return (sbyte)PlayerPrefs.GetInt(key);

                case TypeCode.Int16:
                    return (short)PlayerPrefs.GetInt(key);

                case TypeCode.Int32:
                    return PlayerPrefs.GetInt(key);

                case TypeCode.Int64:
                {
                    string str = PlayerPrefs.GetString(key);
                    return long.Parse(str);
                }

                case TypeCode.Byte:
                    return (byte)PlayerPrefs.GetInt(key);

                case TypeCode.UInt16:
                    return (ushort)PlayerPrefs.GetInt(key);

                case TypeCode.UInt32:
                    return (uint)PlayerPrefs.GetInt(key);

                case TypeCode.UInt64:
                {
                    string str = PlayerPrefs.GetString(key);
                    return ulong.Parse(str);
                }

                case TypeCode.Single:
                    return PlayerPrefs.GetFloat(key);

                case TypeCode.Double:
                {
                    string str = PlayerPrefs.GetString(key);
                    return double.Parse(str, CultureInfo.InvariantCulture);
                }

                case TypeCode.Decimal:
                {
                    string str = PlayerPrefs.GetString(key);
                    return decimal.Parse(str, CultureInfo.InvariantCulture);
                }

                case TypeCode.Char:
                    return (char)PlayerPrefs.GetInt(key);

                case TypeCode.Boolean:
                    return PlayerPrefs.GetInt(key) > 0;

                case TypeCode.String:
                    return PlayerPrefs.GetString(key);

                case TypeCode.DateTime:
                {
                    string str = PlayerPrefs.GetString(key);
                    return new DateTime(long.Parse(str));
                }

                case TypeCode.Object:
                    return JsonUtility.FromJson(PlayerPrefs.GetString(key), type);

                default:
                    throw new UnsupportedValueException(typeCode);
            }
        }
    }
}
