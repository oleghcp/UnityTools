﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityUtility.CSharp.Collections;
using UnityUtility.IO;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.MenuItems
{
    internal static class MenuItemsUtility
    {
        public static void RemoveEmptyFolders()
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

        public static IList<string> SearchReferencesViaDataBase(string targetGuid)
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

        public static IList<string> SearchReferencesInAssetsViaText(string targetGuid)
        {
            return SearchReferencesViaText(AssetDatabaseExt.EnumerateAssetFiles(), targetGuid, IsValidExtension);
        }

        public static IList<string> SearchReferencesInSettingsViaText(string targetGuid)
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
            return extension == AssetDatabaseExt.ASSET_EXTENSION ||
                   extension == AssetDatabaseExt.PREFAB_EXTENSION ||
                   extension == ".unity" ||
                   extension == ".mat" ||
                   extension == ".spriteatlas" ||
                   extension == ".spriteatlasv2" ||
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