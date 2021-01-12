using System.IO;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
    internal static class AssetOpenEditor
    {
        [OnOpenAsset]
        private static bool OpenScriptableObjectClass(int instanceID, int _)
        {
            UnityObject obj = EditorUtility.InstanceIDToObject(instanceID);

            if (obj is ScriptableObject)
            {
                SerializedObject serializedObject = new SerializedObject(obj);
                SerializedProperty prop = serializedObject.FindProperty("m_Script");
                string filePath = AssetDatabase.GetAssetPath(prop.objectReferenceValue);
                System.Diagnostics.Process.Start("devenv", "/edit " + filePath);
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
