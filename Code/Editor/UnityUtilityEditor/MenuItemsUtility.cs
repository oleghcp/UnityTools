using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
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
            float progress = 0f;
            string[] assetGuids = AssetDatabase.FindAssets(string.Empty);

            yield return progress += 0.1f;

            for (int i = 0; i < assetGuids.Length; i++)
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

                yield return Mathf.Lerp(progress, 1f, (i + 1f) / assetGuids.Length);
            }
        }

        public static IEnumerator<float> SearchReferencesByText(string targetGuid, List<string> foundObjects)
        {
            float progress = 0f;

            string projectFolderPath = PathUtility.GetParentPath(Application.dataPath);
            List<string> assets = AssetDatabaseExt.GetFilesFromAssetFolder("*", SearchOption.AllDirectories);

            yield return progress += 0.1f;

            string projectSettingsPath = Path.Combine(projectFolderPath, AssetDatabaseExt.PROJECT_SETTINGS_FOLDER);
            assets.AddRange(Directory.EnumerateFiles(projectSettingsPath, $"*{AssetDatabaseExt.ASSET_EXTENSION}"));

            yield return progress += 0.05f;

            for (int i = 0; i < assets.Count; i++)
            {
                string filePath = assets[i];

                if (invalidExtension(Path.GetExtension(filePath)))
                    continue;

                string text = File.ReadAllText(filePath);

                if (text.Contains(targetGuid))
                {
                    string assetPath = filePath.Substring(projectFolderPath.Length + 1);
                    string guid = AssetDatabase.AssetPathToGUID(assetPath);
                    foundObjects.Add(guid);
                }

                yield return Mathf.Lerp(progress, 1f, (i + 1f) / assets.Count);
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
                       extension != ".overrideController" &&
                       extension != ".mask" &&
                       extension != ".spriteatlas" &&
                       extension != ".playable" &&
                       extension != ".scenetemplate" &&
                       extension != ".asmdef" &&
                       extension != ".terrainlayer" &&
                       extension != ".mixer" &&
                       extension != ".shadervariants" &&
                       extension != ".guiskin" &&
                       extension != ".scenetemplate";
            }
        }

        public static IEnumerator<float> SearchFilesBySize(long minSizeInBytes, List<(UnityObject, long)> foundObjects)
        {
            float progress = 0f;

            List<string> files = AssetDatabaseExt.GetFilesFromAssetFolder("*", SearchOption.AllDirectories);
            string projectFolderPath = PathUtility.GetParentPath(Application.dataPath);

            yield return progress += 0.1f;

            for (int i = 0; i < files.Count; i++)
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

                yield return Mathf.Lerp(progress, 1f, (i + 1f) / files.Count);
            }
        }
    }
}
