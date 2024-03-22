using System;
using System.IO;
using OlegHcp.CSharp;
using OlegHcp.IO;
using OlegHcpEditor.Engine;
using OlegHcpEditor.Utils;
using OlegHcpEditor.Window;
using OlegHcpEditor.Window.ShapeWizards;
using UnityEditor;
using UnityEngine;
using Window;

namespace OlegHcpEditor.MenuItems
{
    internal static class BaseMenuItems
    {
        public const string MAIN_MENU_NAME = "Tools/" + LibConstants.LIB_NAME + "/";

        [MenuItem(MAIN_MENU_NAME + "About", false, 1)]
        private static void GetAboutWindow()
        {
            EditorWindow.GetWindow(typeof(AboutWindow), true, "About");
        }

        [MenuItem(MAIN_MENU_NAME + "Assets/Meshes/Create Rect Plane")]
        private static void GetCreateRectPlaneWizard()
        {
            ScriptableWizard.DisplayWizard("Create Rect Plane", typeof(CreateRectPlaneWizard));
        }

        [MenuItem(MAIN_MENU_NAME + "Assets/Meshes/Create Figured Plane")]
        private static void GetCreateFiguredPlaneWizard()
        {
            ScriptableWizard.DisplayWizard("Create Figured Plane", typeof(CreateFiguredPlaneWizard));
        }

        [MenuItem(MAIN_MENU_NAME + "Assets/Meshes/Create Shape")]
        private static void GetCreateShapeWizard()
        {
            ScriptableWizard.DisplayWizard("Create Shape", typeof(CreateShapeWizard));
        }

        [MenuItem(MAIN_MENU_NAME + "Assets/Find Asset By Guid")]
        private static void FindAssetByGuid()
        {
            EditorWindow.GetWindow(typeof(FindAssetByGuidWindow), false, "Find Asset");
        }

        [MenuItem(MAIN_MENU_NAME + "Assets/Create Scriptable Object Asset")]
        private static void GetScriptableObjectWindow()
        {
            EditorWindow.GetWindow(typeof(CreateAssetWindow), true, "Create Asset");
        }

        [MenuItem(MAIN_MENU_NAME + "Terminal/Create Terminal Prefab")]
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

        [MenuItem(MAIN_MENU_NAME + "Code/Generate Layer Set Class")]
        private static void GenerateLayerSetClass()
        {
            LayerSetWindow.CreateWindow();
        }

        [MenuItem(MAIN_MENU_NAME + "Folders/Open Project Folder")]
        private static void OpenProjectFolder()
        {
            EditorUtilityExt.OpenFolder(PathUtility.GetParentPath(Application.dataPath));
        }

        [MenuItem(MAIN_MENU_NAME + "Folders/Open Persistent Data Folder")]
        private static void OpenPersistentDataFolder()
        {
            EditorUtilityExt.OpenFolder(Application.persistentDataPath);
        }

        [MenuItem(MAIN_MENU_NAME + "Folders/Remove Empty Folders")]
        private static void RemoveEmptyFolders()
        {
            MenuItemsUtility.RemoveEmptyFolders();
        }

        [MenuItem(MAIN_MENU_NAME + "Files/Convert Code Files to UTF8")]
        private static void ConvertCodeFilesToUtf8()
        {
            AssetDatabaseExt.ConvertCodeFilesToUtf8();
        }

        [MenuItem(MAIN_MENU_NAME + "Files/Convert Text Files to UTF8")]
        private static void ConvertTextFilesToUtf8()
        {
            AssetDatabaseExt.ConvertTextFilesToUtf8();
        }

        [MenuItem(MAIN_MENU_NAME + "Files/Find Huge Files")]
        private static void FindHugeFiles()
        {
            SearchHugeFilesWindow.Create();
        }

        [MenuItem(MAIN_MENU_NAME + "Addressables/Analysis Results")]
        private static void OpenAddressablesAnalysisResultsWindow()
        {
#if INCLUDE_ADDRESSABLES && INCLUDE_NEWTONSOFT_JSON
            EditorWindow.GetWindow<AddressablesAnalysisResultsWindow>(false, "Analysis Results", true);
#else
            EditorWindow.GetWindow<AddressablesAnalysisInfo>(true, "Analysis Results", true);
#endif
        }
        [MenuItem(MAIN_MENU_NAME + "Capture Screen/Take Screenshot")]
        private static void Screenshot()
        {
            ScreenCapture.CaptureScreenshot($"Screenshot_{DateTime.Now:yyyy.MM.dd_HH.mm.ss}.png");
        }

        [MenuItem(MAIN_MENU_NAME + "Capture Screen/Take Screenshot as")]
        private static void ScreenshotAs()
        {
            string path = EditorUtility.SaveFilePanel("Take Screenshot", Application.dataPath, $"Screenshot_{DateTime.Now:yyyy.MM.dd_HH.mm.ss}", "png");
            if (path.HasUsefulData())
                ScreenCapture.CaptureScreenshot(path);
        }
    }
}
