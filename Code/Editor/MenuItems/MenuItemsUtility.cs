using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OlegHcp.CSharp.Collections;
using OlegHcp.IO;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.MenuItems
{
    public static class MenuItemsUtility
    {
        public const string CONTEXT_MENU_NAME = "CONTEXT/";
        public const string RESET_ITEM_NAME = "Reset";

        internal static void RemoveEmptyFolders()
        {
            int counter = 0;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Removed folders:");

            removeEmptyFolders(Application.dataPath);

            if (counter == 0)
                EditorUtility.DisplayDialog("Empty Folders Search", "There are no empty folders.", "Ok");
            else
                Debug.Log(builder.ToString());

            void removeEmptyFolders(string path)
            {
                const char forbiddenChar = '.';

                foreach (string directory in Directory.EnumerateDirectories(path))
                {
                    if (Path.GetFileName(directory)[0] != forbiddenChar)
                        removeEmptyFolders(directory);
                }

                if (Directory.EnumerateFileSystemEntries(path).IsNullOrEmpty())
                {
                    string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                    AssetDatabase.DeleteAsset(relativePath);
                    builder.AppendLine(relativePath);
                    counter++;
                }
            }
        }

        internal static IList<string> SearchReferencesViaDataBase(string targetGuid)
        {
            List<string> foundObjects = new List<string>();

            string targetAssetPath = AssetDatabase.GUIDToAssetPath(targetGuid);
            string projectFolderPath = PathUtility.GetParentPath(Application.dataPath);

            AssetDatabaseExt.EnumerateAssetFiles()
                            .Where(fullPath => IsValidExtension(Path.GetExtension(fullPath)))
                            .Select(fullPath => fullPath.Substring(projectFolderPath.Length + 1))
                            .ForEach(checkDependencies);

            void checkDependencies(string assetPath)
            {
                foreach (string dependencyPath in AssetDatabase.GetDependencies(assetPath, false))
                {
                    if (dependencyPath == targetAssetPath && dependencyPath != assetPath)
                    {
                        foundObjects.Add(assetPath);
                        return;
                    }
                }
            }

            return foundObjects;
        }

        internal static IList<string> SearchReferencesInAssetsViaText(string targetGuid)
        {
            return SearchReferencesViaText(AssetDatabaseExt.EnumerateAssetFiles(), targetGuid, IsValidExtension);
        }

        internal static IList<string> SearchReferencesInSettingsViaText(string targetGuid)
        {
            return SearchReferencesViaText(AssetDatabaseExt.EnumerateSettingsFiles(), targetGuid, isValidExtension);

            bool isValidExtension(string extension)
            {
                return extension == AssetDatabaseExt.ASSET_EXTENSION ||
                       extension == ".json";
            }
        }

        private static IList<string> SearchReferencesViaText(IEnumerable<string> enumerable, string targetGuid, Predicate<string> extensionChecker)
        {
            string projectFolderPath = PathUtility.GetParentPath(Application.dataPath);

            return enumerable.AsParallel()
                             .Where(fullPath => extensionChecker(Path.GetExtension(fullPath)) && File.ReadAllText(fullPath).Contains(targetGuid))
                             .Select(fullPath => fullPath.Substring(projectFolderPath.Length + 1))
                             .ToArray();
        }

        private static bool IsValidExtension(string extension)
        {
            switch (extension)
            {
                case AssetDatabaseExt.ASSET_EXTENSION:
                case AssetDatabaseExt.PREFAB_EXTENSION:
                case ".unity":
                case ".mat":
                case ".spriteatlas":
                case ".spriteatlasv2":
                case ".controller":
                case ".overrideController":
                case ".preset":
                case ".mask":
                case ".playable":
                case ".guiskin":
                case ".scenetemplate":
                case ".terrainlayer":
                case ".shadervariants":
                case ".shadergraph":
                case ".asmdef":
                    return true;

                default:
                    return false;
            }
        }
    }
}
