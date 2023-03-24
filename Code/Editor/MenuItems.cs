using System.IO;
using UnityEditor;
using UnityEngine;
using UnityUtility.CSharp;
using UnityUtility.IO;
using UnityUtility.Tools;
using UnityUtilityEditor.Engine;
using UnityUtilityEditor.Window;
using UnityUtilityEditor.Window.ShapeWizards;

namespace UnityUtilityEditor
{
    internal static class MenuItems
    {
        public const string MAIN_MENU_NAME = "Tools/" + LibConstants.LIB_NAME + "/";
        public const string CONTEXT_MENU_NAME = "CONTEXT/";
        public const string RESET_ITEM_NAME = "Reset";

        [MenuItem(MAIN_MENU_NAME + "About", false, 1)]
        private static void GetAboutWindow()
        {
            EditorWindow.GetWindow(typeof(AboutWindow), true, "About");
        }

        [MenuItem(MAIN_MENU_NAME + "Objects/Meshes/Create Rect Plane")]
        private static void GetCreateRectPlaneWizard()
        {
            ScriptableWizard.DisplayWizard("Create Rect Plane", typeof(CreateRectPlaneWizard));
        }

        [MenuItem(MAIN_MENU_NAME + "Objects/Meshes/Create Figured Plane")]
        private static void GetCreateFiguredPlaneWizard()
        {
            ScriptableWizard.DisplayWizard("Create Figured Plane", typeof(CreateFiguredPlaneWizard));
        }

        [MenuItem(MAIN_MENU_NAME + "Objects/Meshes/Create Shape")]
        private static void GetCreateShapeWizard()
        {
            ScriptableWizard.DisplayWizard("Create Shape", typeof(CreateShapeWizard));
        }

        [MenuItem(MAIN_MENU_NAME + "Objects/Create Scriptable Object Asset")]
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

#if INCLUDE_ADDRESSABLES && INCLUDE_NEWTONSOFT_JSON
        [MenuItem(MAIN_MENU_NAME + "Addressables/Analysis Results")]
        private static void OpenAddressablesAnalysisResultsWindow()
        {
            EditorWindow.GetWindow<AddressablesAnalysisResultsWindow>(false, "Analysis Results", true);
        }
#endif
        [MenuItem(MAIN_MENU_NAME + "Capture Screen/Take Screenshot")]
        private static void Screenshot()
        {
            ScreenCapture.CaptureScreenshot($"Screenshot_{Helper.GetDateTimeString()}.png");
        }

        [MenuItem(MAIN_MENU_NAME + "Capture Screen/Take Screenshot as")]
        private static void ScreenshotAs()
        {
            string path = EditorUtility.SaveFilePanel("Take Screenshot", Application.dataPath, $"Screenshot_{Helper.GetDateTimeString()}", "png");
            if (path.HasUsefulData())
                ScreenCapture.CaptureScreenshot(path);
        }
    }
}
