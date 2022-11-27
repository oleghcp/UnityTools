using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityUtility.CSharp;
using UnityUtility.CSharp.Collections;
using UnityUtility.CSharp.IO;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Engine
{
    public static class AssetDatabaseExt
    {
        public const string ASSET_FOLDER = "Assets/";
        public const string PROJECT_SETTINGS_FOLDER = "ProjectSettings/";
        public const string ASSET_EXTENSION = ".asset";

        public static string FullPathToProjectRelative(string fullPath)
        {
            return ASSET_FOLDER + fullPath.Substring(Application.dataPath.Length + 1);
        }

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

        public static T LoadAssetByGuid<T>(string guid) where T : UnityObject
        {
            return LoadAssetByGuid(guid, typeof(T)) as T;
        }

        public static UnityObject LoadAssetByGuid(string guid, Type type)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath(path, type);
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
            return EnumerateFiles(Application.dataPath, searchPattern);
        }

        public static IEnumerable<string> EnumerateSettingsFiles()
        {
            return EnumerateSettingsFiles("*");
        }

        public static IEnumerable<string> EnumerateSettingsFiles(string searchPattern)
        {
            string projectFolderPath = PathUtility.GetParentPath(Application.dataPath);
            string projectSettingsPath = Path.Combine(projectFolderPath, PROJECT_SETTINGS_FOLDER);

            return EnumerateFiles(projectSettingsPath, searchPattern);
        }

        public static void ConvertToUtf8()
        {
            IEnumerable<string> files = enumerateFiles("*.txt").Join(enumerateFiles("*.xml"))
                                                               .Join(enumerateFiles("*.json"))
                                                               .Join(enumerateFiles("*.cs"))
                                                               .Join(enumerateFiles("*.shader"))
                                                               .Join(enumerateFiles("*.cginc"));
            foreach (string filePath in files)
            {
                string text = File.ReadAllText(filePath);
                File.WriteAllText(filePath, text, Encoding.UTF8);
            }
            AssetDatabase.Refresh();

            IEnumerable<string> enumerateFiles(string pattern)
            {
                return Directory.EnumerateFiles(Application.dataPath, pattern, SearchOption.AllDirectories);
            }
        }

        private static IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            const char forbiddenChar = '.';

            return enumerate(path);

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
