using OlegHcp.NodeBased.Service;
using OlegHcpEditor.Configs;
using OlegHcpEditor.Engine;
using OlegHcpEditor.Window;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor
{
#pragma warning disable IDE0051
    internal static class AssetOpenEditor
    {
        [OnOpenAsset]
        private static bool OpenScriptableObjectClass(int instanceID, int _)
        {
#if UNITY_6000_3_OR_NEWER
            UnityObject obj = EditorUtility.EntityIdToObject(instanceID);
#else
            UnityObject obj = EditorUtility.InstanceIDToObject(instanceID);
#endif

            if (obj is RawGraph graphAsset)
            {
                GraphEditorWindow.Open(graphAsset);
                return true;
            }

            if (obj is ScriptableObject scriptableObject && OlegHcpUserSettings.OpenScriptableAssetsAsCode)
            {
                EditorUtilityExt.OpenScriptableObjectCode(scriptableObject);
                return true;
            }

            if (ProjectWindowUtil.IsFolder(instanceID) && OlegHcpUserSettings.OpenFoldersByDoubleClick)
            {
                EditorUtilityExt.OpenFolder(obj.GetAssetPath());
                return true;
            }

            return false;
        }
    }
}
