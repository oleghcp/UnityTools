using UnityEditor;
using UnityUtility.Async;

#if UNITY_2018_3_OR_NEWER
namespace UnityUtilityEditor.Async
{
    [CustomEditor(typeof(AsyncSystemSettings))]
    public class AsyncSystemSettingsEditor : Editor
    {
        private readonly string HELP_BOX_NOTE = $"Don't touch this asset. Use {TaskSystem.SYSTEM_NAME} section in Project Settings instead.";

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(HELP_BOX_NOTE, MessageType.Warning);
        }
    }
}
#endif
