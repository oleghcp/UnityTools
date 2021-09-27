using UnityEditor;
using UnityUtility.Async;

#if UNITY_2018_3_OR_NEWER
namespace UnityUtilityEditor.Inspectors.AsyncSystem
{
    [CustomEditor(typeof(AsyncSystemSettings))]
    internal class AsyncSystemSettingsEditor : Editor
    {
        private readonly string HELP_BOX_NOTE = $"Use {TaskSystem.SYSTEM_NAME} section in Project Settings to edit this asset.";

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(HELP_BOX_NOTE, MessageType.Warning);
        }
    }
}
#endif
