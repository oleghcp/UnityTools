using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityUtility.CSharp;
using UnityUtility.IO;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Engine
{
    public static class AssetDatabaseExt
    {
        public const string ASSET_FOLDER = "Assets/";
        public const string PROJECT_SETTINGS_FOLDER = "ProjectSettings/";
        public const string USER_SETTINGS_FOLDER = "UserSettings/";
        public const string LIBRARY_FOLDER = "Library/";
        public const string ASSET_EXTENSION = ".asset";
        public const string PREFAB_EXTENSION = ".prefab";

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
            return EnumerateScriptAssemblies().ToArray();
        }

        public static IEnumerable<Assembly> EnumerateScriptAssemblies()
        {
            return Directory.EnumerateFiles($"{LIBRARY_FOLDER}ScriptAssemblies/", "*.dll", SearchOption.AllDirectories)
                            .Select(Assembly.LoadFrom);
        }

        public static void ConvertCodeFilesToUtf8()
        {
            ConvertToUtf8("*.cs", "*.shader", "*.cginc");
        }

        public static void ConvertTextFilesToUtf8()
        {
            ConvertToUtf8("*.txt", "*.xml", "*.json", "*.cs", "*.shader", "*.cginc");
        }

        public static void ConvertToUtf8(params string[] extensions)
        {
            string dataPath = Application.dataPath;

            extensions.SelectMany(pattern => EnumerateFiles(dataPath, pattern))
                      .AsParallel()
                      .ForAll(convert);

            AssetDatabase.Refresh();

            void convert(string filePath)
            {
                string text = File.ReadAllText(filePath);
                File.WriteAllText(filePath, text, Encoding.UTF8);
            }
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

        private static IEnumerable<string> EnumerateFiles(string directoryPath, string searchPattern)
        {
            const char forbiddenChar = '.';

            foreach (string filePath in Directory.EnumerateFiles(directoryPath, searchPattern))
            {
                if (Path.GetFileName(filePath)[0] != forbiddenChar)
                    yield return filePath;
            }

            foreach (string dirPath in Directory.EnumerateDirectories(directoryPath))
            {
                if (Path.GetFileName(dirPath)[0] == forbiddenChar)
                    continue;

                foreach (string filePath in EnumerateFiles(dirPath, searchPattern))
                {
                    yield return filePath;
                }
            }
        }
    }
}
