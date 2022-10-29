using UnityEditor.Callbacks;
using UnityEngine;
#if UNITY_2019_3_OR_NEWER
using UnityUtility.NodeBased;
using UnityUtilityEditor.Window;
#endif
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
    internal static class AssetOpenEditor
    {
#pragma warning disable IDE0051
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
            if (obj is ScriptableObject scriptableObject)
            {
                EditorUtilityExt.OpenScriptableObjectCode(scriptableObject);
                return true;
            }

            if (ProjectWindowUtil.IsFolder(instanceID))
            {
                EditorUtilityExt.OpenFolder(obj.GetAssetPath());
                return true;
            }

            return false;
        }
    }
}
