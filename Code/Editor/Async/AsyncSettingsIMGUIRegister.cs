using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtility.Async;

#if UNITY_2018_3_OR_NEWER
internal static class AsyncSettingsIMGUIRegister
{
    private static GUIContent s_labelForStopField;
    private static GUIContent s_labelForGlobalStopField;
    private static GUIContent s_labelForDestoryField;

    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {
        s_labelForStopField = new GUIContent("Allow stop tasks",
            $"Option for providing possibility to stop each task manually using {nameof(TaskInfo)}.{nameof(TaskInfo.Stop)}() or {nameof(TaskInfo)}.{nameof(TaskInfo.SkipCurrent)}()");

        s_labelForGlobalStopField = new GUIContent("Allow stop all tasks",
            "Option for providing possibility to stop all tasks globally by registering object with global stopping event.");

        s_labelForDestoryField = new GUIContent("Don't destroy on load",
            "Whether tasks runners should be destroyed when scene unloaded.");

        return new SettingsProvider("Project/" + TaskSystem.SYSTEM_NAME, SettingsScope.Project)
        {
            guiHandler = f_drawGui,
            keywords = new HashSet<string> { "Async", "Async System", "Stop", "Destroy" },
        };
    }

    private static void f_drawGui(string searchContext)
    {
        var settings = AsyncSystemSettings.GetSerializedObject();

        var property = settings.FindProperty(AsyncSystemSettings.CanBeStoppedName);

        property.boolValue = EditorGUILayout.Toggle(s_labelForStopField, property.boolValue);

        GUI.enabled = property.boolValue;

        EditorGUILayout.PropertyField(settings.FindProperty(AsyncSystemSettings.CanBeStoppedGloballyName), s_labelForGlobalStopField);
        EditorGUILayout.PropertyField(settings.FindProperty(AsyncSystemSettings.DontDestroyOnLoadName), s_labelForDestoryField);

        GUI.enabled = true;

        settings.ApplyModifiedProperties();
    }
}
#endif
