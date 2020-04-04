using UnityEditor;

internal static class AsyncSettingsIMGUIRegister
{
    public const string ASYNC_SYSTEM_SETTINGS_NAME = "Async System Settings";

    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {
        return new SettingsProvider("Project/" + ASYNC_SYSTEM_SETTINGS_NAME, SettingsScope.Project)
        {
            guiHandler = searchContext =>
            {
                //var settings = MyCustomSettings.GetSerializedSettings();
                //EditorGUILayout.PropertyField(settings.FindProperty("m_Number"), new GUIContent("My Number"));
                //EditorGUILayout.PropertyField(settings.FindProperty("m_SomeString"), new GUIContent("My String"));
                EditorGUILayout.HelpBox("Blank gui for Async settings.", MessageType.Info);
            },

            //keywords = new HashSet<string>(new[] { "Number", "Some String" })
        };
    }
}
