using System.Collections.Generic;
using UnityEditor;
using UnityUtility.Async;

#if UNITY_2018_3_OR_NEWER
internal static class AsyncSettingsIMGUIRegister
{
    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {
        return new SettingsProvider("Project/" + Tasks.SYSTEM_NAME, SettingsScope.Project)
        {
            guiHandler = f_drawGui,
            keywords = new HashSet<string> { "Number", "Some String" },
        };
    }

    private static void f_drawGui(string searchContext)
    {
        var settings = AsyncSystemSettings.GetSerializedObject();

        SerializedProperty iterator = settings.GetIterator();

        for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
        {
            if (iterator.propertyPath == "m_Script")
                continue;

            EditorGUILayout.PropertyField(iterator, true);
        }

        settings.ApplyModifiedProperties();
    }
}
#endif
