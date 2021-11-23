using System.IO;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityUtility.NodeBased;
using UnityUtilityEditor.Window.NodeBased;
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
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

            if (obj is ScriptableObject)
            {
                EditorUtilityExt.OpenScriptableObjectCode(obj);
                return true;
            }

            if (ProjectWindowUtil.IsFolder(instanceID))
            {
                DirectoryInfo dir = Directory.CreateDirectory(AssetDatabase.GetAssetPath(obj));
                System.Diagnostics.Process.Start(dir.FullName);
                return true;
            }

            return false;
        }
    }
}
