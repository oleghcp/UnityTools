﻿using System.IO;
using UnityEditor;
using UnityEngine;
using UnityUtility.CSharp.IO;
using UnityUtilityEditor.Engine;
using UnityUtilityEditor.Window;
using UnityUtilityEditor.Window.ShapeWizards;

namespace UnityUtilityEditor
{
    internal static class MenuItems
    {
        public const string CONTEXT_MENU_NAME = "CONTEXT/";
        public const string RESET_ITEM_NAME = "Reset";

        [MenuItem(LibConstants.LIB_NAME + "/Objects/Meshes/Create Rect Plane")]
        private static void GetCreateRectPlaneWizard()
        {
            ScriptableWizard.DisplayWizard("Create Rect Plane", typeof(CreateRectPlaneWizard));
        }

        [MenuItem(LibConstants.LIB_NAME + "/Objects/Meshes/Create Figured Plane")]
        private static void GetCreateFiguredPlaneWizard()
        {
            ScriptableWizard.DisplayWizard("Create Figured Plane", typeof(CreateFiguredPlaneWizard));
        }

        [MenuItem(LibConstants.LIB_NAME + "/Objects/Meshes/Create Shape")]
        private static void GetCreateShapeWizard()
        {
            ScriptableWizard.DisplayWizard("Create Shape", typeof(CreateShapeWizard));
        }

        [MenuItem(LibConstants.LIB_NAME + "/Objects/Create Scriptable Object Asset")]
        private static void GetScriptableObjectWindow()
        {
            EditorWindow.GetWindow(typeof(CreateAssetWindow), true, "Create Asset");
        }

        [MenuItem(LibConstants.LIB_NAME + "/Terminal/Create Terminal Prefab")]
        private static void CreateTerminalPrefab()
        {
            string assetPath = AssetDatabase.GUIDToAssetPath("7537be9dac6f69045a2656580dc951f0");
            string newFolderPath = $"{AssetDatabaseExt.ASSET_FOLDER}{nameof(Resources)}";

            Directory.CreateDirectory(newFolderPath);
            GameObject prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath(assetPath);
            prefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            PrefabUtility.SaveAsPrefabAsset(prefab, $"{newFolderPath}/Terminal{AssetDatabaseExt.PREFAB_EXTENSION}");
            prefab.DestroyImmediate();
            AssetDatabase.SaveAssets();
        }

        [MenuItem(LibConstants.LIB_NAME + "/Code/Generate Layer Set Class")]
        private static void GenerateLayerSetClass()
        {
            LayerSetWindow.CreateWindow();
        }

        [MenuItem(LibConstants.LIB_NAME + "/Folders/Open Project Folder")]
        private static void OpenProjectFolder()
        {
            EditorUtilityExt.OpenFolder(PathUtility.GetParentPath(Application.dataPath));
        }

        [MenuItem(LibConstants.LIB_NAME + "/Folders/Open Persistent Data Folder")]
        private static void OpenPersistentDataFolder()
        {
            EditorUtilityExt.OpenFolder(Application.persistentDataPath);
        }

        [MenuItem(LibConstants.LIB_NAME + "/Folders/Remove Empty Folders")]
        private static void RemoveEmptyFolders()
        {
            MenuItemsUtility.RemoveEmptyFolders();
        }

        [MenuItem(LibConstants.LIB_NAME + "/Files/Convert Code Files to UTF8")]
        private static void ConvertCodeFilesToUtf8()
        {
            AssetDatabaseExt.ConvertCodeFilesToUtf8();
        }

        [MenuItem(LibConstants.LIB_NAME + "/Files/Convert Text Files to UTF8")]
        private static void ConvertTextFilesToUtf8()
        {
            AssetDatabaseExt.ConvertTextFilesToUtf8();
        }

        [MenuItem(LibConstants.LIB_NAME + "/Files/Find Huge Files")]
        private static void FindHugeFiles()
        {
            SearchHugeFilesWindow.Create();
        }

#if UNITY_2019_3_OR_NEWER && INCLUDE_ADDRESSABLES && INCLUDE_NEWTONSOFT_JSON
        [MenuItem(LibConstants.LIB_NAME + "/Addressables/Analysis Results")]
        private static void OpenAddressablesAnalysisResultsWindow()
        {
            EditorWindow.GetWindow<AddressablesAnalysisResultsWindow>(false, "Analysis Results", true);
        }
#endif

        [MenuItem(LibConstants.LIB_NAME + "/About", false, 1)]
        private static void GetAboutWindow()
        {
            EditorWindow.GetWindow(typeof(AboutWindow), true, "About");
        }

#if LIBRARY_EDIT
        [MenuItem(nameof(UnityUtility) + "/Library Editor Window")]
        private static void Open()
        {
            EditorWindow.GetWindow<LibraryEditorWindow>();
        }
#endif
    }
}
