﻿using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityUtility.Async;

#if UNITY_2018_3_OR_NEWER
namespace UnityUtilityEditor.SettingsProviders
{
    internal static class AsyncSettingsProvider
    {
        private static GUIContent _labelForStopField;
        private static GUIContent _labelForGlobalStopField;
        private static GUIContent _labelForDestoryField;

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            string typeName = nameof(TaskInfo);

            _labelForStopField = new GUIContent("Allow stop tasks",
                $"Option for providing possibility to stop each task manually using {typeName}.{nameof(TaskInfo.Stop)}() or {typeName}.{nameof(TaskInfo.SkipCurrent)}()");

            _labelForGlobalStopField = new GUIContent("Allow stop all tasks",
                "Option for providing possibility to stop all tasks globally by registering object with global stopping event.");

            _labelForDestoryField = new GUIContent("Don't destroy on load",
                "Whether task runners should be destroyed when scene is unloaded.");

            return new SettingsProvider("Project/" + TaskSystem.SYSTEM_NAME, SettingsScope.Project)
            {
                guiHandler = DrawGui,
                keywords = new HashSet<string> { "Async", "Async System", "Stop", "Destroy" },
            };
        }

        private static void DrawGui(string searchContext)
        {
            using (SerializedObject settings = new SerializedObject(GetOrCreateSettings()))
            {
                SerializedProperty canBeStoppedProperty = settings.FindProperty(AsyncSystemSettings.CanBeStoppedName);
                SerializedProperty canBeStoppedGloballyProperty = settings.FindProperty(AsyncSystemSettings.CanBeStoppedGloballyName);

                canBeStoppedProperty.boolValue = EditorGUILayout.Toggle(_labelForStopField, canBeStoppedProperty.boolValue);

                GUI.enabled = canBeStoppedProperty.boolValue;

                canBeStoppedGloballyProperty.boolValue = EditorGUILayout.Toggle(_labelForGlobalStopField, canBeStoppedGloballyProperty.boolValue);

                if (canBeStoppedGloballyProperty.boolValue)
                {
                    EditorGUILayout.HelpBox("Are you sure? Stopping all tasks globally is a dangerous practice.", MessageType.Warning);
                }

                GUI.enabled = true;

                settings.ApplyModifiedProperties();
                canBeStoppedProperty.Dispose();
                canBeStoppedGloballyProperty.Dispose();
            }
        }

        private static AsyncSystemSettings GetOrCreateSettings()
        {
            string settingsPath = $"{EditorUtilityExt.ASSET_FOLDER}{nameof(Resources)}/{nameof(AsyncSystemSettings)}{EditorUtilityExt.ASSET_EXTENSION}";

            var settings = AssetDatabase.LoadAssetAtPath<AsyncSystemSettings>(settingsPath);
            if (settings == null)
            {
                string path = Path.Combine(Application.dataPath, nameof(Resources));
                Directory.CreateDirectory(path);
                settings = ScriptableObject.CreateInstance<AsyncSystemSettings>();
                AssetDatabase.CreateAsset(settings, settingsPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }
    }
}
#endif
