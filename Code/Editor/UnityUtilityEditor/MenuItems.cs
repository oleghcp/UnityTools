using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityUtilityEditor.Inspectors;
using UnityUtilityEditor.Window;
using UnityUtilityEditor.Window.ShapeWizards;

namespace UnityUtilityEditor
{
    internal static class MenuItems
    {
        public const string CONTEXT_MENU_NAME = "CONTEXT/";
        public const string RESET_ITEM_NAME = "Reset";

        [MenuItem(nameof(UnityUtility) + "/Objects/Meshes/Create Rect Plane")]
        private static void GetCreateRectPlaneWizard()
        {
            ScriptableWizard.DisplayWizard("Create Rect Plane", typeof(CreateRectPlaneWizard));
        }

        [MenuItem(nameof(UnityUtility) + "/Objects/Meshes/Create Figured Plane")]
        private static void GetCreateFiguredPlaneWizard()
        {
            ScriptableWizard.DisplayWizard("Create Figured Plane", typeof(CreateFiguredPlaneWizard));
        }

        [MenuItem(nameof(UnityUtility) + "/Objects/Meshes/Create Shape")]
        private static void GetCreateShapeWizard()
        {
            ScriptableWizard.DisplayWizard("Create Shape", typeof(CreateShapeWizard));
        }

        [MenuItem(nameof(UnityUtility) + "/Objects/Create Scriptable Object Asset")]
        private static void GetScriptableObjectWindow()
        {
            EditorWindow.GetWindow(typeof(CreateAssetWindow), true, "Create Asset");
        }

        [MenuItem(nameof(UnityUtility) + "/Terminal/Create Terminal Prefab")]
        private static void CreateTerminal()
        {
            string assetPath = AssetDatabase.GUIDToAssetPath("7537be9dac6f69045a2656580dc951f0");
            string newFolderPath = $"{AssetDatabaseExt.ASSET_FOLDER}{nameof(Resources)}";

            Directory.CreateDirectory(newFolderPath);
            GameObject prefab = PrefabUtility.LoadPrefabContents(assetPath);
            PrefabUtility.SaveAsPrefabAsset(prefab, $"{newFolderPath}/Terminal.prefab");
            AssetDatabase.SaveAssets();
        }

        [MenuItem(nameof(UnityUtility) + "/Code/Generate Layer Set Class")]
        private static void GenerateLayerSetClass()
        {
            LayerSetWindow.CreateWindow();
        }

        [MenuItem(nameof(UnityUtility) + "/Folders/Open Project Folder")]
        private static void OpenProjectSettingsFolder()
        {
            EditorUtilityExt.OpenFolder(PathUtility.GetParentPath(Application.dataPath));
        }

        [MenuItem(nameof(UnityUtility) + "/Folders/Open Persistent Data Folder")]
        private static void OpenPersistentDataFolder()
        {
            EditorUtilityExt.OpenFolder(Application.persistentDataPath);
        }

        [MenuItem(nameof(UnityUtility) + "/Folders/Remove Empty Folders")]
        private static void RemoveEmptyFolders()
        {
            MenuItemsUtility.RemoveEmptyFolders();
        }

        [MenuItem(nameof(UnityUtility) + "/Files/Convert .cs Files to UTF8")]
        private static void ConvertToUtf8()
        {
            foreach (var filePath in Directory.EnumerateFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories))
            {
                string text = File.ReadAllText(filePath);
                File.WriteAllText(filePath, text, Encoding.UTF8);
            }
            AssetDatabase.Refresh();
        }

        [MenuItem(nameof(UnityUtility) + "/Files/Find Huge Files")]
        private static void FindHugeFiles()
        {
            SearchHugeFilesWindow.Create();
        }

        [MenuItem(nameof(UnityUtility) + "/Switch Transform Editor")]
        private static void SwitchTransformEditor()
        {
            TransformEditor.SwitchType();
        }

#if !UNITY_2018_3_OR_NEWER
        [MenuItem(nameof(UnityUtility) + "/Gamepad Axes")]
        private static void GamepadAxes()
        {
            GamepadAxesWindow.Create();
        }
#endif

        [MenuItem(nameof(UnityUtility) + "/About", false, 1)]
        private static void GetAboutWindow()
        {
            EditorWindow.GetWindow(typeof(AboutWindow), true, "About");
        }
    }
}
