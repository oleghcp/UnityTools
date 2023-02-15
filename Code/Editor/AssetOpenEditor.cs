using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityUtilityEditor.Engine;
using UnityUtilityEditor.Configs;
#if UNITY_2019_3_OR_NEWER
using UnityUtility.NodeBased;
using UnityUtilityEditor.Window;
#endif
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor
{
#pragma warning disable IDE0051
    internal static class AssetOpenEditor
    {
        [OnOpenAsset]
        private static bool OpenScriptableObjectClass(int instanceID, int _)
        {
            UnityObject obj = EditorUtility.InstanceIDToObject(instanceID);

#if UNITY_2019_3_OR_NEWER
            if (obj is RawGraph graphAsset)
            {
                GraphEditorWindow.Open(graphAsset);
                return true;
            }
#endif
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
