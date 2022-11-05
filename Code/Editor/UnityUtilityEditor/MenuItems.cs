using System.IO;
using UnityEditor;
using UnityEngine;
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
        private static void CreateTerminal()
        {
            string assetPath = AssetDatabase.GUIDToAssetPath("7537be9dac6f69045a2656580dc951f0");
            string newFolderPath = $"{AssetDatabaseExt.ASSET_FOLDER}{nameof(Resources)}";

            Directory.CreateDirectory(newFolderPath);
            GameObject prefab = PrefabUtility.LoadPrefabContents(assetPath);
            PrefabUtility.SaveAsPrefabAsset(prefab, $"{newFolderPath}/Terminal.prefab");
            AssetDatabase.SaveAssets();
        }

        [MenuItem(LibConstants.LIB_NAME + "/Code/Generate Layer Set Class")]
        private static void GenerateLayerSetClass()
        {
            LayerSetWindow.CreateWindow();
        }

        [MenuItem(LibConstants.LIB_NAME + "/Folders/Open Project Folder")]
        private static void OpenProjectSettingsFolder()
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

        [MenuItem(LibConstants.LIB_NAME + "/Files/Convert text Files to UTF8")]
        private static void ConvertToUtf8()
        {
            AssetDatabaseExt.ConvertToUtf8();
        }

        [MenuItem(LibConstants.LIB_NAME + "/Files/Find Huge Files")]
        private static void FindHugeFiles()
        {
            SearchHugeFilesWindow.Create();
        }

        [MenuItem(LibConstants.LIB_NAME + "/About", false, 1)]
        private static void GetAboutWindow()
        {
            EditorWindow.GetWindow(typeof(AboutWindow), true, "About");
        }
    }
}
