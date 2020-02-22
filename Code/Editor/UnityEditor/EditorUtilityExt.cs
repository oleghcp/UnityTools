using UnityObject = UnityEngine.Object;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using UnityEngine;

namespace UnityEditor
{
    public static class EditorUtilityExt
    {
        public static UnityObject LoadAssetByGuid(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath(path, typeof(UnityObject));
        }

        public static bool FindReferences(string targetGuid, List<string> referingObjectGuids)
        {
            string[] assetGuids = AssetDatabase.FindAssets(string.Empty);

            bool noDependencies = true;

            for (int i = 0; i < assetGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
                string[] dependencies = AssetDatabase.GetDependencies(assetPath);

                foreach (var dependency in dependencies)
                {
                    string dependencyGuid = AssetDatabase.AssetPathToGUID(dependency);

                    if (dependencyGuid == targetGuid && dependencyGuid != assetGuids[i])
                    {
                        noDependencies = false;
                        referingObjectGuids.Add(assetGuids[i]);
                    }
                }
            }

            return !noDependencies;
        }

        public static Assembly[] GetAssemblies()
        {
            var files = Directory.GetFiles(@"Library\ScriptAssemblies\", "*.dll", SearchOption.AllDirectories);

            return files.Select(file => Assembly.LoadFrom(file)).ToArray();
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

        public static void CreateScriptableObjectAsset(string objectName, string fileName = null)
        {
            ScriptableObject so = ScriptableObject.CreateInstance(objectName);
            string name = string.Concat("Assets/", fileName.HasUsefulData() ? fileName : objectName, ".asset");
            AssetDatabase.CreateAsset(so, name);
            AssetDatabase.SaveAssets();
        }

        public static void CreateScriptableObjectAsset(string objectName, UnityObject rootObject)
        {
            ScriptableObject so = ScriptableObject.CreateInstance(objectName);
            so.name = objectName;
            AssetDatabase.AddObjectToAsset(so, rootObject);
            AssetDatabase.SaveAssets();
        }
    }
}
