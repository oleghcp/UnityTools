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
            List<string> foundObjects = new List<string>();

            string targetAssetPath = AssetDatabase.GUIDToAssetPath(targetGuid);
            string projectFolderPath = PathUtility.GetParentPath(Application.dataPath);

            IEnumerable<string> assets = AssetDatabaseExt.EnumerateAssetFiles()
                                                         .Where(item => IsValidExtension(Path.GetExtension(item)))
                                                         .Select(item => item.Substring(projectFolderPath.Length + 1));
            foreach (string assetPath in assets)
            {
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
            string projectFolderPath = PathUtility.GetParentPath(Application.dataPath);
            string projectSettingsPath = Path.Combine(projectFolderPath, AssetDatabaseExt.PROJECT_SETTINGS_FOLDER);

            IEnumerable<string> assets = AssetDatabaseExt.EnumerateAssetFiles();
            IEnumerable<string> settingsAssets = Directory.EnumerateFiles(projectSettingsPath, $"*{AssetDatabaseExt.ASSET_EXTENSION}");

            return assets.Concat(settingsAssets)
                         .AsParallel()
                         .Where(item => IsValidExtension(Path.GetExtension(item)) && File.ReadAllText(item).Contains(targetGuid))
                         .Select(item => item.Substring(projectFolderPath.Length + 1))
                         .ToArray();
        }

        private static bool IsValidExtension(string extension)
        {
            return extension == AssetDatabaseExt.ASSET_EXTENSION ||
                   extension == ".unity" ||
                   extension == ".prefab" ||
                   extension == ".mat" ||
                   extension == ".spriteatlas" ||
                   extension == ".controller" ||
                   extension == ".overrideController" ||
                   extension == ".preset" ||
                   extension == ".mask" ||
                   extension == ".playable" ||
                   extension == ".guiskin" ||
                   extension == ".scenetemplate" ||
                   extension == ".terrainlayer" ||
                   extension == ".shadervariants";
        }
    }
}
