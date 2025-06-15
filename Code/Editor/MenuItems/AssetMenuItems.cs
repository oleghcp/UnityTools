using System;
using System.Collections.Generic;
using OlegHcp.CSharp;
using OlegHcp.CSharp.Collections;
using OlegHcpEditor.Utils;
using OlegHcpEditor.Window;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor.MenuItems
{
    internal static class AssetMenuItems
    {
        public const string MENU_PARENT = AssetDatabaseExt.ASSET_FOLDER;
        public const string CREATE_ASSET_PATH = MENU_PARENT + "Create/" + LibConstants.LIB_NAME + "/Asset";

        private const string FULL_MENU_GRAPH_PATH = MENU_PARENT + "Create/" + LibConstants.LIB_NAME + "/Graph/";

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

        [MenuItem(CREATE_ASSET_PATH)]
        private static void CreateAsset()
        {
            CreateAssetWindow.Create(true);
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

            return AssetDatabaseExt.LoadAssetByGuid<UnityObject>(selectedGuids[0]) != Selection.activeObject;
        }

        [MenuItem(MENU_PARENT + "Find References In Project (ext.)/Via Asset Database", false, 26)]
        private static void FindReferencesViaAssetDatabase()
        {
            FindReferences(MenuItemsUtility.SearchReferencesViaDataBase);
        }

        [MenuItem(MENU_PARENT + "Find References In Project (ext.)/Via Text Searching", false, 26)]
        private static void FindReferencesViaTextSearching()
        {
            FindReferences(MenuItemsUtility.SearchReferencesInAssetsViaText);
        }

        [MenuItem(MENU_PARENT + "Find References In Settings (ext.)", false, 26)]
        private static void FindReferencesInSettingsViaTextSearching()
        {
            FindReferences(MenuItemsUtility.SearchReferencesInSettingsViaText);
        }

        private static void FindReferences(Func<string, IList<string>> searcher)
        {
            string targetGuid = Selection.assetGUIDs[0];
            IList<string> collection = searcher(targetGuid);

            if (collection.IsNullOrEmpty())
            {
                EditorUtility.DisplayDialog("References Search", "There are no references.", "Ok");
                return;
            }

            ReferencesWindow.Create(targetGuid, collection);
        }

        [MenuItem(MENU_PARENT + "Find References In Project (ext.)/Via Text Searching", true)]
        [MenuItem(MENU_PARENT + "Find References In Settings (ext.)", true)]
        private static bool FindReferencesViaTextSearchingValidate()
        {
            return EditorSettings.serializationMode == SerializationMode.ForceText;
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
#else
        private const int CREATE_CS_SCRIPT_PRIORITY = 81;
#endif

        [MenuItem(MENU_PARENT + "Create/C# Script (ext.)", false, CREATE_CS_SCRIPT_PRIORITY)]
        private static void CreateScript()
        {
            TemplatesUtility.CreateScript();
        }
    }
}
