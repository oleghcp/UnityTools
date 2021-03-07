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
    public static class EditorUtilityExt
    {
        public const string SCRIPT_FIELD = "m_Script";
        public const string ASSET_NAME_FIELD = "m_Name";
        public const string ASSET_EXTENSION = ".asset";
        public const string ASSET_FOLDER = "Assets/";

        private static MethodInfo s_clearFunc;

        public static UnityObject LoadAssetByGuid(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath(path, typeof(UnityObject));
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
