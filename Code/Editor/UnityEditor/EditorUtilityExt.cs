using UnityObject = UnityEngine.Object;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using UnityEngine;
using UnityUtility.MathExt;
using System.Collections;

namespace UnityEditor
{
    public static class EditorUtilityExt
    {
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

            string projectFolderPath = PathExt.GetParentPath(assetsFolderPath);
            int count = files.Length;
            int actionsPerFrame = count.Cbrt().ToInt().CutBefore(1);

            yield return null;

            for (int i = 0; i < count; i++)
            {
                progress.Progress = (i + 1f) / count;

                string filePath = files[i];
                string extension = Path.GetExtension(filePath);

                bool invalid = extension != ".prefab" &&
                              extension != ".unity" &&
                              extension != ".asset" &&
                              extension != ".mat" &&
                              extension != ".preset" &&
                              extension != ".anim" &&
                              extension != ".controller" &&
                              extension != ".spriteatlas" &&
                              !extension.Contains("override");

                if (invalid)
                {
                    continue;
                }

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
