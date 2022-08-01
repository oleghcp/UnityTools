using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtilityEditor.Window;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor
{
    internal static class AssetMenuItems
    {
        public const string MENU_PARENT = AssetDatabaseExt.ASSET_FOLDER;
        public const string CREATE_ASSET_PATH = MENU_PARENT + "Create/" + nameof(UnityUtility) + "/Asset";

#if UNITY_2019_3_OR_NEWER
        private const string FULL_MENU_GRAPH_PATH = MENU_PARENT + "Create/" + nameof(UnityUtility) + "/Graph/";

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
            CreateAssetWindow window = EditorWindow.GetWindow<CreateAssetWindow>(true, "Scriptable Objects");
            window.SetParent(Selection.activeObject);
        }

        [MenuItem(CREATE_ASSET_PATH, true)]
        private static bool CreateAssetValidation()
        {
            return Selection.objects.Length == 1;
        }

        [MenuItem(MENU_PARENT + "Destroy (ext.)", false, 20)]
        private static void DestroySubasset()
        {
            UnityObject.DestroyImmediate(Selection.activeObject, true);
            AssetDatabase.SaveAssets();
        }

        [MenuItem(MENU_PARENT + "Destroy (ext.)", true)]
        private static bool DestroySubassetValidation()
        {
            string[] selectedGuids = Selection.assetGUIDs;

            if (Selection.objects.Length != 1 || selectedGuids.Length != 1)
                return false;

            return AssetDatabaseExt.LoadAssetByGuid(selectedGuids[0]) != Selection.activeObject;
        }

        [MenuItem(MENU_PARENT + "Find References In Project (ext.)", false, 25)]
        private static void FindReferences()
        {
            string targetGuid = Selection.assetGUIDs[0];
            List<string> foundObjects = new List<string>();

            EditorUtilityExt.ExecuteWithProgressBarCancelable("Searching references",
                                                       "That could take a while...",
                                                       getIterator(),
                                                       () => ReferencesWindow.Create(targetGuid, foundObjects));
            IEnumerator<float> getIterator()
            {
                if (EditorSettings.serializationMode == SerializationMode.ForceText)
                    return MenuItemsUtility.SearchReferencesByText(targetGuid, foundObjects);

                return MenuItemsUtility.SearchReferencesByDataBase(targetGuid, foundObjects);
            }
        }

        [MenuItem(MENU_PARENT + "Find References In Project (ext.)", true)]
        private static bool FindRefsValidation()
        {
            return Selection.assetGUIDs.Length == 1;
        }

        [MenuItem(MENU_PARENT + "Show Guid (ext.)", false, 20)]
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
        [MenuItem(MENU_PARENT + "Create/C# Script (ext.)", false, CREATE_CS_SCRIPT_PRIORITY)]
        private static void CreateScript()
        {
            TemplatesUtility.CreateScript();
        }
#endif
    }
}
