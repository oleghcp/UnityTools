using UnityEditor;
using UnityEngine;
using UnityUtilityEditor.Window;
using UnityUtilityEditor.Window.ShapeWizards;

namespace UnityUtilityEditor
{
    internal static class MenuItems
    {
#if UNITY_2019_3_OR_NEWER
        [MenuItem(EditorUtilityExt.ASSET_FOLDER + "Create/Graph (ext.)/Node C# Script")]
        private static void CreateNodeScript()
        {
            GraphAssetMenuUtility.CreateNodeScript();
        }

        [MenuItem(EditorUtilityExt.ASSET_FOLDER + "Create/Graph (ext.)/Transition C# Script")]
        private static void CreateTransitionScript()
        {
            GraphAssetMenuUtility.CreateTransitionScript();
        }

        [MenuItem(EditorUtilityExt.ASSET_FOLDER + "Create/Graph (ext.)/Graph C# Script")]
        private static void CreateGraphScript()
        {
            GraphAssetMenuUtility.CreateGraphScript();
        }
#endif

        [MenuItem(nameof(UnityUtility) + "/Remove Empty Folders")]
        private static void RemoveEmptyFolders()
        {
            MenuItemsUtility.RemoveEmptyFolders();
        }

        [MenuItem(nameof(UnityUtility) + "/Input/Gamepad Axes")]
        private static void GetAxisCreateWindow()
        {
            EditorWindow.GetWindow(typeof(AxisCreateWindow), true, "Gamepad Axes");
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

        [MenuItem(EditorUtilityExt.ASSET_FOLDER + "Find References In Project (ext.)", false, 25)]
        private static void FindReferences()
        {
            MenuItemsUtility.FindReferences();
        }

        [MenuItem(EditorUtilityExt.ASSET_FOLDER + "Find References In Project (ext.)", true)]
        private static bool FindRefsValidation()
        {
            return Selection.assetGUIDs.Length == 1;
        }

        [MenuItem(EditorUtilityExt.ASSET_FOLDER + "Show Guid (ext.)", false, 20)]
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
        [MenuItem(EditorUtilityExt.ASSET_FOLDER + "Create/C# Script (ext.)", false, CREATE_CS_SCRIPT_PRIORITY)]
        private static void CreateScript()
        {
            MenuItemsUtility.CreateScript();
        }
#endif
    }
}
