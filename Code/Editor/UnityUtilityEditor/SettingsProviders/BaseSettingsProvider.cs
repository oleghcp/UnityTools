using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.SettingsProviders
{
    internal class BaseSettingsProvider : SettingsProvider
    {
        private GUILayoutOption _labelWidth = GUILayout.Width(250f);

        public BaseSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        private static SettingsProvider CreateProvider()
        {
            return new BaseSettingsProvider($"{SettingsScope.Project}/{LibConstants.LIB_NAME}", SettingsScope.Project);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.Space();

            DrawToggle(PrefsKeys.OPEN_FOLDERS_BY_CLICK, "Open Folders by Double Click");
            DrawToggle(PrefsKeys.OPEN_SO_ASSETS_CODE_BY_CLICK, "Open ScriptableObject Assets as Code");
        }

        private void DrawToggle(string key, string label)
        {
            bool curValue = EditorPrefs.GetBool(key);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(label, _labelWidth);
            bool newValue = EditorGUILayout.Toggle(curValue);
            EditorGUILayout.EndHorizontal();

            if (newValue != curValue)
                EditorPrefs.SetBool(key, newValue);
        }
    }
}
