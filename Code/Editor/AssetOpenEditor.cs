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
#if UNITY_6000_4_OR_NEWER
        private static bool OpenScriptableObjectClass(EntityId entityId, int _)
#else
        private static bool OpenScriptableObjectClass(int instanceID, int _)
#endif
        {
#if UNITY_6000_4_OR_NEWER
            UnityObject obj = EditorUtility.EntityIdToObject(entityId);
#elif UNITY_6000_3_OR_NEWER
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

#if UNITY_6000_4_OR_NEWER
            if (ProjectWindowUtil.IsFolder(entityId) && OlegHcpUserSettings.OpenFoldersByDoubleClick)
#else
            if (ProjectWindowUtil.IsFolder(instanceID) && OlegHcpUserSettings.OpenFoldersByDoubleClick)
#endif
            {
                EditorUtilityExt.OpenFolder(obj.GetAssetPath());
                return true;
            }

            return false;
        }
    }
}
