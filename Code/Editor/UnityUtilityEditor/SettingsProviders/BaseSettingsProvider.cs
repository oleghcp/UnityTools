using System.Collections.Generic;
using UnityEditor;

namespace UnityUtilityEditor.SettingsProviders
{
    internal class BaseSettingsProvider : SettingsProvider
    {
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

            EditorGUIUtility.labelWidth = 250f;
            DrawToggle(PrefsKeys.OPEN_FOLDERS_BY_CLICK, "Open Folders by Double Click");
            DrawToggle(PrefsKeys.OPEN_SO_ASSETS_CODE_BY_CLICK, "Open ScriptableObject Assets as Code");
            DrawText(PrefsKeys.SUPPRESSED_WARNINGS_IN_IDE, "Suppressed Warnings in IDE", "If you specify multiple warning numbers, separate them with a comma. \n Example: CS0649,CS0169");
        }

        private void DrawToggle(string key, string label, string tooltip = null)
        {
            bool curValue = EditorPrefs.GetBool(key);
            bool newValue = EditorGUILayout.Toggle(EditorGuiUtility.TempContent(label, tooltip), curValue);
            if (newValue != curValue)
                EditorPrefs.SetBool(key, newValue);
        }

        private void DrawText(string key, string label, string tooltip = null)
        {
            string curValue = EditorPrefs.GetString(key);
            string newValue = EditorGUILayout.TextField(EditorGuiUtility.TempContent(label, tooltip), curValue);
            if (newValue != curValue)
                EditorPrefs.SetString(key, newValue);
        }
    }
}
