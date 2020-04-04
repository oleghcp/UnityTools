using UnityEditor;
using UnityUtility.Async;

namespace UnityUtilityEditor.Async
{
    [CustomEditor(typeof(AsyncSystemSettings))]
    public class AsyncSystemSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox($"Don't touch this asset. Use {AsyncSettingsIMGUIRegister.ASYNC_SYSTEM_SETTINGS_NAME} in Project Settings instead.",
                                    MessageType.Warning);
        }
    }
}
