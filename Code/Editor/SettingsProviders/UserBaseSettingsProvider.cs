using System.Collections.Generic;
using UnityEditor;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.SettingsProviders
{
    internal class UserBaseSettingsProvider : SettingsProvider
    {
        public UserBaseSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        private static SettingsProvider CreateProvider()
        {
            return new UserBaseSettingsProvider($"{SettingsProviderUtility.USER_SECTION_NAME}/{LibConstants.LIB_NAME}", SettingsScope.User);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.Space();

            EditorGUIUtility.labelWidth = SettingsProviderUtility.LABEL_WIDTH;
            if (DrawToggle(PrefsKeys.OPEN_FOLDERS_BY_CLICK, "Open Folders by Double Click"))
                EditorGUILayout.HelpBox("Folders opening  by double click works only with one column layout of Project window.", MessageType.Info);
            DrawToggle(PrefsKeys.OPEN_SO_ASSETS_CODE_BY_CLICK, "Open ScriptableObject Assets as Code");

            DrawText(PrefsKeys.SUPPRESSED_WARNINGS_IN_IDE,
                     "Suppressed Warnings in IDE",
                     tooltip: "If you specify multiple warning numbers, separate them with a comma. \n Example: CS0649,CS0169",
                     defaultValue: CsprojFilePostprocessor.DEFAULT_WARNING);
        }

        private bool DrawToggle(string key, string label)
        {
            bool curValue = EditorPrefs.GetBool(key);
            bool newValue = EditorGUILayout.Toggle(EditorGuiUtility.TempContent(label), curValue);
            if (newValue != curValue)
                EditorPrefs.SetBool(key, newValue);
            return newValue;
        }

        private void DrawText(string key, string label, string tooltip = null, string defaultValue = null)
        {
            string curValue = EditorPrefs.GetString(key, defaultValue);
            string newValue = EditorGUILayout.TextField(EditorGuiUtility.TempContent(label, tooltip), curValue);
            if (newValue != curValue)
                EditorPrefs.SetString(key, newValue);
        }
    }
}
