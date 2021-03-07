using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtilityEditor.Window;

namespace UnityUtilityEditor
{
    internal static class MenuItemsUtility
    {
        public static void RemoveEmptyFolders()
        {
            removeEmptyFolders(Application.dataPath);

            Debug.Log("Done");

            void removeEmptyFolders(string path)
            {
                IEnumerable<string> directories = Directory.EnumerateDirectories(path);

                foreach (string directory in directories)
                {
                    removeEmptyFolders(directory);
                }

                IEnumerable<string> entries = Directory.EnumerateFileSystemEntries(path);

                if (entries.Any()) { return; }

                string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                AssetDatabase.DeleteAsset(relativePath);

                Debug.Log("Deleted: " + relativePath);
            }
        }

        public static void FindReferences()
        {
            string targetGuid = Selection.assetGUIDs[0];
            SearchProgress progress = new SearchProgress();
            bool isCanseled = false;
            IEnumerator iterator;

            if (EditorSettings.serializationMode == SerializationMode.ForceText)
            {
                iterator = FindReferencesByText(targetGuid, progress);
            }
            else
            {
                iterator = FindReferencesDataBase(targetGuid, progress);
            }

            EditorApplication.update += Proceed;

            void Proceed()
            {
                if (!isCanseled && iterator.MoveNext())
                {
                    isCanseled = EditorUtility.DisplayCancelableProgressBar("Searching references", "That could take a while...", progress.Progress);
                    return;
                }

                EditorApplication.update -= Proceed;

                EditorUtility.ClearProgressBar();

                if (progress.FoundObjects.Count == 0)
                {
                    Debug.Log("There are no dependencies.");
                    return;
                }

                ReferencesWindow.Create(targetGuid, progress.FoundObjects);
            }
        }

        private static IEnumerator FindReferencesDataBase(string targetGuid, SearchProgress progress)
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

        private static IEnumerator FindReferencesByText(string targetGuid, SearchProgress progress)
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
                return extension != EditorUtilityExt.ASSET_EXTENSION &&
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

#if UNITY_2019_1_OR_NEWER        
        public static void CreateScript()
        {
            string templatePath = Path.Combine(PathUtility.GetParentPath(Application.dataPath), "C#ScriptTemplate.cs.txt");

            if (!File.Exists(templatePath))
            {
                string text = @"using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityUtility;
using UnityObject = UnityEngine.Object;

namespace Project
{
    public class #SCRIPTNAME# : MonoBehaviour
    {

    }
}
";
                File.WriteAllText(templatePath, text);
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyClass.cs");
        }
#endif

        private class SearchProgress
        {
            public List<object> FoundObjects = new List<object>();
            public float Progress;
        }
    }
}
