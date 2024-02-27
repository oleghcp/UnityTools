using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using OlegHcp.NodeBased.Service;
using OlegHcpEditor.Configs;
using OlegHcpEditor.Engine;
using OlegHcpEditor.Window;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor
{
#pragma warning disable IDE0051
    internal static class AssetOpenEditor
    {
        [OnOpenAsset]
        private static bool OpenScriptableObjectClass(int instanceID, int _)
        {
            UnityObject obj = EditorUtility.InstanceIDToObject(instanceID);

            if (obj is RawGraph graphAsset)
            {
                GraphEditorWindow.Open(graphAsset);
                return true;
            }

            if (obj is ScriptableObject scriptableObject && LibrarySettings.OpenScriptableAssetsAsCode)
            {
                EditorUtilityExt.OpenScriptableObjectCode(scriptableObject);
                return true;
            }

            if (ProjectWindowUtil.IsFolder(instanceID) && LibrarySettings.OpenFoldersByDoubleClick)
            {
                EditorUtilityExt.OpenFolder(obj.GetAssetPath());
                return true;
            }

            return false;
        }
    }
}
