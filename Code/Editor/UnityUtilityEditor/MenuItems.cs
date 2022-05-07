using System.IO;
using UnityEditor;
using UnityEngine;
using UnityUtilityEditor.Window;
using UnityUtilityEditor.Window.ShapeWizards;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor
{
    internal static class MenuItems
    {
        public const string CONTEXT_MENU_NAME = "CONTEXT/";
        public const string RESET_ITEM_NAME = "Reset";
        public const string CREATE_ASSET_PATH = AssetDatabaseExt.ASSET_FOLDER + "Create/" + nameof(UnityUtility) + "/Asset";

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

        [MenuItem(nameof(UnityUtility) + "/Open Persistent Data Folder")]
        private static void OpenPersistentDataFolder()
        {
            DirectoryInfo dir = Directory.CreateDirectory(Application.persistentDataPath);
            System.Diagnostics.Process.Start(dir.FullName);
        }

#if !UNITY_2018_3_OR_NEWER
        [MenuItem(nameof(UnityUtility) + "/Gamepad Axes")]
        private static void GamepadAxes()
        {
            GamepadAxesWindow.Create();
        }
#endif

        [MenuItem(nameof(UnityUtility) + "/Find Huge Files")]
        private static void FindHugeFiles()
        {
            SearchHugeFilesWindow.Create();
        }

        [MenuItem(nameof(UnityUtility) + "/Remove Empty Folders")]
        private static void RemoveEmptyFolders()
        {
            MenuItemsUtility.RemoveEmptyFolders();
        }

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

        [MenuItem(nameof(UnityUtility) + "/Objects/Create Scriptable Object")]
        private static void GetScriptableObjectWindow()
        {
            EditorWindow.GetWindow(typeof(ScriptableObjectWindow), true, "Scriptable Objects");
        }

        // ------------- //

        [MenuItem(nameof(UnityUtility) + "/About", false, 1)]
        private static void GetAboutWindow()
        {
            EditorWindow.GetWindow(typeof(AboutWindow), true, "About");
        }

        // ------------- //

#if UNITY_2019_3_OR_NEWER
        private const string FULL_MENU_GRAPH_PATH = AssetDatabaseExt.ASSET_FOLDER + "Create/" + nameof(UnityUtility) + "/Graph/";

        [MenuItem(FULL_MENU_GRAPH_PATH + "Node C# Script")]
        private static void CreateNodeScript()
        {
            TemplatesUtility.CreateNodeScript();
        }

        [MenuItem(FULL_MENU_GRAPH_PATH + "Graph C# Script")]
        private static void CreateGraphScript()
        {
            TemplatesUtility.CreateGraphScript();
        }
#endif

        [MenuItem(CREATE_ASSET_PATH)]
        private static void CreateAsset()
        {
            ScriptableObjectWindow window = EditorWindow.GetWindow<ScriptableObjectWindow>(true, "Scriptable Objects");
            window.SetParent(Selection.activeObject);
        }

        [MenuItem(CREATE_ASSET_PATH, true)]
        private static bool CreateAssetValidation()
        {
            return Selection.objects.Length == 1;
        }

        [MenuItem(AssetDatabaseExt.ASSET_FOLDER + "Destroy (ext.)", false, 20)]
        private static void DestroySubasset()
        {
            UnityObject.DestroyImmediate(Selection.activeObject, true);
            AssetDatabase.SaveAssets();
        }

        [MenuItem(AssetDatabaseExt.ASSET_FOLDER + "Destroy (ext.)", true)]
        private static bool DestroySubassetValidation()
        {
            string[] selectedGuids = Selection.assetGUIDs;

            if (Selection.objects.Length != 1 || selectedGuids.Length != 1)
                return false;

            return AssetDatabaseExt.LoadAssetByGuid(selectedGuids[0]) != Selection.activeObject;
        }

        [MenuItem(AssetDatabaseExt.ASSET_FOLDER + "Find References In Project (ext.)", false, 25)]
        private static void FindReferences()
        {
            if (EditorSettings.serializationMode == SerializationMode.ForceText)
                MenuItemsUtility.FindReferences(MenuItemsUtility.SearchReferencesByText);
            else
                MenuItemsUtility.FindReferences(MenuItemsUtility.SearchReferencesByDataBase);
        }

        [MenuItem(AssetDatabaseExt.ASSET_FOLDER + "Find References In Project (ext.)", true)]
        private static bool FindRefsValidation()
        {
            return Selection.assetGUIDs.Length == 1;
        }

        [MenuItem(AssetDatabaseExt.ASSET_FOLDER + "Show Guid (ext.)", false, 20)]
        private static void ShowGUID()
        {
            string[] guids = Selection.assetGUIDs;

            for (int i = 0; i < guids.Length; i++)
            {
                Debug.Log(guids[i]);
            }
        }

#if UNITY_2020_2_OR_NEWER
        private const int CREATE_CS_SCRIPT_PRIORITY = 80;
#elif UNITY_2019_1_OR_NEWER
        private const int CREATE_CS_SCRIPT_PRIORITY = 81;
#endif

#if UNITY_2019_1_OR_NEWER
        [MenuItem(AssetDatabaseExt.ASSET_FOLDER + "Create/C# Script (ext.)", false, CREATE_CS_SCRIPT_PRIORITY)]
        private static void CreateScript()
        {
            TemplatesUtility.CreateScript();
        }
#endif
    }
}
