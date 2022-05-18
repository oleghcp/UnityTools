using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
    public static class AssetDatabaseExt
    {
        public const string ASSET_FOLDER = "Assets/";
        public const string PROJECT_SETTINGS_FOLDER = "ProjectSettings/";
        public const string ASSET_EXTENSION = ".asset";
        internal const string TEMPLATES_FOLDER = "Templates/";

        public static void CreateScriptableObjectAsset(Type type, string assetPath = null)
        {
            ScriptableObject so = ScriptableObject.CreateInstance(type);
            string name = assetPath.IsNullOrWhiteSpace() ? $"{ASSET_FOLDER}{type.Name}{ASSET_EXTENSION}"
                                                         : assetPath;
            AssetDatabase.CreateAsset(so, name);
            AssetDatabase.SaveAssets();
        }

        public static void CreateScriptableObjectAsset(Type type, UnityObject rootObject, string assetName = null)
        {
            ScriptableObject so = ScriptableObject.CreateInstance(type);
            so.name = assetName.IsNullOrWhiteSpace() ? type.Name : assetName;
            AssetDatabase.AddObjectToAsset(so, rootObject);
            AssetDatabase.SaveAssets();
        }

        public static UnityObject LoadAssetByGuid(string guid, Type type)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath(path, type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T LoadAssetByGuid<T>(string guid) where T : UnityObject
        {
            return LoadAssetByGuid(guid, typeof(T)) as T;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnityObject LoadAssetByGuid(string guid)
        {
            return LoadAssetByGuid(guid, typeof(UnityObject));
        }

        public static Assembly[] LoadScriptAssemblies()
        {
            return Directory.GetFiles(@"Library\ScriptAssemblies\", "*.dll", SearchOption.AllDirectories)
                            .Select(file => Assembly.LoadFrom(file))
                            .ToArray();
        }

        public static List<string> GetFilesFromAssetFolder(string searchPattern, SearchOption searchOption)
        {
            const char forbiddenChar = '.';

            List<string> list = new List<string>();
            search(Application.dataPath);
            return list;

            void search(string path)
            {
                foreach (string directory in Directory.EnumerateDirectories(path))
                {
                    if (Path.GetFileName(directory)[0] == forbiddenChar)
                        continue;

                    foreach (string file in Directory.EnumerateFiles(directory, searchPattern))
                    {
                        if (Path.GetFileName(file)[0] == forbiddenChar)
                            continue;

                        list.Add(file);
                    }

                    if (searchOption == SearchOption.AllDirectories)
                        search(directory);
                }
            }
        }
    }
}
