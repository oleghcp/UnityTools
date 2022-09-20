using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
                        foundObjects.Add(AssetDatabase.GUIDToAssetPath(assetGuids[i]));
                    }
                }

                yield return Mathf.Lerp(progress, 1f, (i + 1f) / assetGuids.Length);
            }
        }

        public static IEnumerator<float> SearchReferencesByText(string targetGuid, List<string> foundObjects)
        {
            HashSet<string> extensions = new HashSet<string>()
            {
                AssetDatabaseExt.ASSET_EXTENSION,
                ".prefab",
                ".unity",
                ".mat",
                ".preset",
                ".controller",
                ".overrideController",
                ".mask",
                ".spriteatlas",
                ".playable",
                ".scenetemplate",
                ".asmdef",
                ".terrainlayer",
                ".mixer",
                ".shadervariants",
                ".guiskin",
            };

            string projectFolderPath = PathUtility.GetParentPath(Application.dataPath);
            string projectSettingsPath = Path.Combine(projectFolderPath, AssetDatabaseExt.PROJECT_SETTINGS_FOLDER);

            IEnumerable<string> assets = AssetDatabaseExt.EnumerateAssetFiles("*");
            IEnumerable<string> settingsAssets = Directory.EnumerateFiles(projectSettingsPath, $"*{AssetDatabaseExt.ASSET_EXTENSION}");

            yield return 0.1f;

            string[] result = assets.Concat(settingsAssets)
                                    .AsParallel()
                                    .Where(item => extensions.Contains(Path.GetExtension(item)) && File.ReadAllText(item).Contains(targetGuid))
                                    .Select(item => item.Substring(projectFolderPath.Length + 1))
                                    .ToArray();

            yield return 0.9f;

            foundObjects.AddRange(result);

            yield return 1f;
        }
    }
}
