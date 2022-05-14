using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtility.MathExt;
using UnityObject = UnityEngine.Object;

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
                foreach (string directory in Directory.EnumerateDirectories(path))
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

        public static IEnumerator<float> SearchReferencesByDataBase(string targetGuid, List<string> foundObjects)
        {
            string[] assetGuids = AssetDatabase.FindAssets(string.Empty);
            int count = assetGuids.Length;
            int actionsPerFrame = count.Cbrt().ToInt().CutBefore(1);

            yield return 0f;

            for (int i = 0; i < count; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
                string[] dependencies = AssetDatabase.GetDependencies(assetPath);

                foreach (string dependency in dependencies)
                {
                    string dependencyGuid = AssetDatabase.AssetPathToGUID(dependency);

                    if (dependencyGuid == targetGuid && dependencyGuid != assetGuids[i])
                    {
                        foundObjects.Add(assetGuids[i]);
                    }
                }

                if (i % actionsPerFrame == 0)
                {
                    yield return (i + 1f) / count;
                }
            }
        }

        public static IEnumerator<float> SearchReferencesByText(string targetGuid, List<string> foundObjects)
        {
            IReadOnlyList<string> files = AssetDatabaseExt.GetFilesFromAssetFolder("*", SearchOption.AllDirectories);

            yield return 0f;

            string projectFolderPath = PathUtility.GetParentPath(Application.dataPath);
            int count = files.Count;
            int actionsPerFrame = count.Cbrt().ToInt().CutBefore(1);

            yield return 0f;

            for (int i = 0; i < count; i++)
            {
                string filePath = files[i];

                if (invalidExtension(Path.GetExtension(filePath)))
                    continue;

                string text = File.ReadAllText(filePath);

                if (text.Contains(targetGuid))
                {
                    string assetPath = filePath.Remove(0, projectFolderPath.Length + 1);
                    string guid = AssetDatabase.AssetPathToGUID(assetPath);
                    foundObjects.Add(guid);
                }

                if (i % actionsPerFrame == 0)
                {
                    yield return (i + 1f) / count;
                }
            }

            bool invalidExtension(string extension)
            {
                return extension != AssetDatabaseExt.ASSET_EXTENSION &&
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

        public static IEnumerator<float> SearchFilesBySize(long minSizeInBytes, List<(UnityObject, long)> foundObjects)
        {
            IReadOnlyList<string> files = AssetDatabaseExt.GetFilesFromAssetFolder("*", SearchOption.AllDirectories);

            yield return 0f;

            string projectFolderPath = PathUtility.GetParentPath(Application.dataPath);
            int count = files.Count;
            int actionsPerFrame = count.Cbrt().ToInt().CutBefore(1);

            yield return 0f;

            for (int i = 0; i < count; i++)
            {
                string filePath = files[i];

                if (Path.GetExtension(filePath) == ".meta")
                    continue;

                FileInfo info = new FileInfo(filePath);

                if (info.Length >= minSizeInBytes)
                {
                    string assetPath = filePath.Remove(0, projectFolderPath.Length + 1);
                    string guid = AssetDatabase.AssetPathToGUID(assetPath);
                    UnityObject asset = AssetDatabaseExt.LoadAssetByGuid<UnityObject>(guid);
                    foundObjects.Add((asset, info.Length));
                }

                if (i % actionsPerFrame == 0)
                {
                    yield return (i + 1f) / count;
                }
            }
        }
    }
}
