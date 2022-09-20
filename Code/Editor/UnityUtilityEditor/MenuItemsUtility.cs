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

        public static IEnumerable<string> SearchReferencesByDataBase(string targetGuid)
        {
            string targetAssetPath = AssetDatabase.GUIDToAssetPath(targetGuid);
            string[] assetsGuids = AssetDatabase.FindAssets(string.Empty);

            List<string> foundObjects = new List<string>();

            for (int i = 0; i < assetsGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetsGuids[i]);

                foreach (string dependencyPath in AssetDatabase.GetDependencies(assetPath, false))
                {
                    if (dependencyPath == targetAssetPath && dependencyPath != assetPath)
                        foundObjects.Add(assetPath);
                }
            }

            return foundObjects;
        }

        public static IEnumerable<string> SearchReferencesByText(string targetGuid)
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

            return assets.Concat(settingsAssets)
                         .AsParallel()
                         .Where(item => extensions.Contains(Path.GetExtension(item)) && File.ReadAllText(item).Contains(targetGuid))
                         .Select(item => item.Substring(projectFolderPath.Length + 1))
                         .ToArray();
        }
    }
}
