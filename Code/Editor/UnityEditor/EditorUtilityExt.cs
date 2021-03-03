using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.MathExt;
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
    public static class EditorUtilityExt
    {
        public const string SCRIPT_FIELD = "m_Script";
        public const string ASSET_NAME_FIELD = "m_Name";
        public const string ASSET_EXTENSION = ".asset";
        public const string ASSET_FOLDER = "Assets/";

        private static MethodInfo s_clearFunc;

        public class SearchProgress
        {
            public List<object> FoundObjects = new List<object>();
            public float Progress;
        }

        public static UnityObject LoadAssetByGuid(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath(path, typeof(UnityObject));
        }

        public static IEnumerator FindReferences(string targetGuid, SearchProgress progress)
        {
            string[] assetGuids = AssetDatabase.FindAssets(string.Empty);
            int count = assetGuids.Length;
            int actionsPerFrame = count.Cbrt().ToInt().CutBefore(1);

            yield return null;

            for (int i = 0; i < count; i++)
            {
                progress.Progress = (i + 1f) / count;

                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
                string[] dependencies = AssetDatabase.GetDependencies(assetPath);

                foreach (string dependency in dependencies)
                {
                    string dependencyGuid = AssetDatabase.AssetPathToGUID(dependency);

                    if (dependencyGuid == targetGuid && dependencyGuid != assetGuids[i])
                    {
                        progress.FoundObjects.Add(assetGuids[i]);
                    }
                }

                if (i % actionsPerFrame == 0)
                {
                    yield return null;
                }
            }
        }

        public static IEnumerator FindReferencesByText(string targetGuid, SearchProgress progress)
        {
            string assetsFolderPath = Application.dataPath;
            string[] files = Directory.GetFiles(assetsFolderPath, "*", SearchOption.AllDirectories);

            yield return null;

            string projectFolderPath = PathUtility.GetParentPath(assetsFolderPath);
            int count = files.Length;
            int actionsPerFrame = count.Cbrt().ToInt().CutBefore(1);

            yield return null;

            for (int i = 0; i < count; i++)
            {
                progress.Progress = (i + 1f) / count;

                string filePath = files[i];

                if (invalidExtension(Path.GetExtension(filePath)))
                    continue;

                string text = File.ReadAllText(filePath);

                if (text.Contains(targetGuid))
                {
                    string assetPath = filePath.Remove(0, projectFolderPath.Length + 1);
                    string guid = AssetDatabase.AssetPathToGUID(assetPath);
                    progress.FoundObjects.Add(guid);
                }

                if (i % actionsPerFrame == 0)
                {
                    yield return null;
                }
            }

            bool invalidExtension(string extension)
            {
                return extension != ASSET_EXTENSION &&
                       extension != ".prefab" &&
                       extension != ".unity" &&
                       extension != ".mat" &&
                       extension != ".preset" &&
                       extension != ".anim" &&
                       extension != ".controller" &&
                       extension != ".spriteatlas" &&
                       extension != ".scenetemplate" &&
                       !extension.Contains("override");
            }
        }

        public static Assembly[] GetAssemblies()
        {
            return Directory.GetFiles(@"Library\ScriptAssemblies\", "*.dll", SearchOption.AllDirectories)
                            .Select(file => Assembly.LoadFrom(file))
                            .ToArray();
        }

        public static Type[] GetTypes(Assembly[] assemblies, Func<Type, bool> selector)
        {
            List<Type> types = new List<Type>();

            for (int i = 0; i < assemblies.Length; i++)
            {
                types.AddRange(assemblies[i].GetTypes());
            }

            return types.Where(selector).ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SaveProject()
        {
            EditorApplication.ExecuteMenuItem("File/Save Project");
        }

        public static void ClearConsoleWindow()
        {
            if (s_clearFunc == null)
            {
                Assembly assembly = Assembly.GetAssembly(typeof(Editor));
                Type type = assembly.GetType("UnityEditor.LogEntries");
                s_clearFunc = type.GetMethod("Clear");
            }
            s_clearFunc.Invoke(null, null);
        }

        public static void CreateScriptableObjectAsset(Type type, string assetName = null)
        {
            ScriptableObject so = ScriptableObject.CreateInstance(type);
            string name = GetAssetName(type, assetName);
            AssetDatabase.CreateAsset(so, name);
            AssetDatabase.SaveAssets();
        }

        public static void CreateScriptableObjectAsset(Type type, UnityObject rootObject, string assetName = null)
        {
            ScriptableObject so = ScriptableObject.CreateInstance(type);
            so.name = GetAssetName(type, assetName);
            AssetDatabase.AddObjectToAsset(so, rootObject);
            AssetDatabase.SaveAssets();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetAssetName(Type type, string path)
        {
            return path.IsNullOrWhiteSpace() ? $"{ASSET_FOLDER}{type.Name}{ASSET_EXTENSION}" : path;
        }

        public static (string AssemblyName, string ClassName) SplitSerializedPropertyTypename(string typename)
        {
            if (typename.IsNullOrEmpty())
                return (null, null);

            string[] typeSplitString = typename.Split(' ');
            return (typeSplitString[0], typeSplitString[1]);
        }

        public static Type GetTypeFromSerializedPropertyTypename(string typename)
        {
            var (assemblyName, className) = SplitSerializedPropertyTypename(typename);
            return Type.GetType($"{className}, {assemblyName}");
        }

        public static Type GetFieldType(FieldInfo fieldInfo)
        {
            return fieldInfo.FieldType.IsArray ? fieldInfo.FieldType.GetElementType()
                                               : fieldInfo.FieldType;
        }
    }
}
