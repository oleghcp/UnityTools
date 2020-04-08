﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtility.Async;

#if UNITY_2018_3_OR_NEWER
namespace UnityUtilityEditor.Async
{
    internal static class AsyncSettingsIMGUIRegister
    {
        private static GUIContent s_labelForStopField;
        private static GUIContent s_labelForGlobalStopField;
        private static GUIContent s_labelForDestoryField;

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            string typeName = nameof(TaskInfo);

            s_labelForStopField = new GUIContent("Allow stop tasks",
                $"Option for providing possibility to stop each task manually using {typeName}.{nameof(TaskInfo.Stop)}() or {typeName}.{nameof(TaskInfo.SkipCurrent)}()");

            s_labelForGlobalStopField = new GUIContent("Allow stop all tasks",
                "Option for providing possibility to stop all tasks globally by registering object with global stopping event.");

            s_labelForDestoryField = new GUIContent("Don't destroy on load",
                "Whether task runners should be destroyed when scene is unloaded.");

            return new SettingsProvider("Project/" + TaskSystem.SYSTEM_NAME, SettingsScope.Project)
            {
                guiHandler = f_drawGui,
                keywords = new HashSet<string> { "Async", "Async System", "Stop", "Destroy" },
            };
        }

        private static void f_drawGui(string searchContext)
        {
            var settings = AsyncSystemSettings.GetSerializedObject();

            var canBeStoppedProperty = settings.FindProperty(AsyncSystemSettings.CanBeStoppedName);
            var canBeStoppedGloballyProperty = settings.FindProperty(AsyncSystemSettings.CanBeStoppedGloballyName);

            canBeStoppedProperty.boolValue = EditorGUILayout.Toggle(s_labelForStopField, canBeStoppedProperty.boolValue);

            GUI.enabled = canBeStoppedProperty.boolValue;

            canBeStoppedGloballyProperty.boolValue = EditorGUILayout.Toggle(s_labelForGlobalStopField, canBeStoppedGloballyProperty.boolValue);

            if (canBeStoppedGloballyProperty.boolValue)
            {
                EditorGUILayout.HelpBox("Are you sure? Stopping all tasks globally is a dangerous practice.", MessageType.Warning);
            }

            GUI.enabled = true;

            settings.ApplyModifiedProperties();
        }
    }
}
#endif
