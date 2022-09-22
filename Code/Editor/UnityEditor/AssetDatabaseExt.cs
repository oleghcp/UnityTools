using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static T LoadAssetByGuid<T>(string guid) where T : UnityObject
        {
            return LoadAssetByGuid(guid, typeof(T)) as T;
        }

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

        public static IEnumerable<string> EnumerateAssetFiles()
        {
            return EnumerateAssetFiles("*");
        }

        public static IEnumerable<string> EnumerateAssetFiles(string searchPattern)
        {
            const char forbiddenChar = '.';

            return enumerate(Application.dataPath);

            IEnumerable<string> enumerate(string directoryPath)
            {
                foreach (string filePath in Directory.EnumerateFiles(directoryPath, searchPattern))
                {
                    if (Path.GetFileName(filePath)[0] != forbiddenChar)
                        yield return filePath;
                }

                foreach (string dirPath in Directory.EnumerateDirectories(directoryPath))
                {
                    if (Path.GetFileName(dirPath)[0] == forbiddenChar)
                        continue;

                    foreach (string filePath in enumerate(dirPath))
                    {
                        yield return filePath;
                    }
                }
            }
        }
    }
}
