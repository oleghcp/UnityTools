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
        public const string SCRIPT_FIELD_NAME = "m_Script";

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
                return extension != ".prefab" &&
                       extension != ".unity" &&
                       extension != ".asset" &&
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect GetLinePosition(in Rect basePosition, int lineIndex)
        {
            return GetLinePosition(basePosition, lineIndex, EditorGUIUtility.singleLineHeight);
        }

        public static Rect GetLinePosition(in Rect basePosition, int lineIndex, float lineHeight)
        {
            float lineSpace = EditorGUIUtility.standardVerticalSpacing;

            float xPos = basePosition.xMin;
            float yPos = basePosition.yMin + (lineHeight + lineSpace) * lineIndex;

            return new Rect(xPos, yPos, basePosition.width, lineHeight);
        }

        public static Rect GetLinePosition(in Rect basePosition, int line, int column, int columnCount)
        {
            float lineSpace = EditorGUIUtility.standardVerticalSpacing;

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float lineWidth = basePosition.width;

            if (columnCount > 1)
            {
                lineWidth -= lineSpace * (columnCount - 1);
                lineWidth /= columnCount;
            }

            float yPos = basePosition.yMin + (lineHeight + lineSpace) * line;
            float xPos = basePosition.xMin;

            if (columnCount > 1)
                xPos += (lineWidth + lineSpace) * column;

            return new Rect(xPos, yPos, lineWidth, lineHeight);
        }
    }
}
