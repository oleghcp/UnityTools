using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityUtility.Async;

#if UNITY_2018_3_OR_NEWER
namespace UnityUtilityEditor.SettingsProviders
{
    internal class AsyncSettingsProvider : SettingsProvider
    {
        private readonly string _settingsPath;

        private readonly GUIContent _labelForStopField;
        private readonly GUIContent _labelForGlobalStopField;
        private readonly GUIContent _labelForDestoryField;

        private SerializedObject _serializedObject;

        public AsyncSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
            _settingsPath = $"{AssetDatabaseExt.ASSET_FOLDER}{nameof(Resources)}/{nameof(AsyncSystemSettings)}{AssetDatabaseExt.ASSET_EXTENSION}";

            string typeName = nameof(TaskInfo);

            _labelForStopField = new GUIContent("Allow stop tasks",
                $"Option for providing possibility to stop each task manually using {typeName}.{nameof(TaskInfo.Stop)}() or {typeName}.{nameof(TaskInfo.SkipCurrent)}()");

            _labelForGlobalStopField = new GUIContent("Allow stop all tasks",
                "Option for providing possibility to stop all tasks globally by registering object with global stopping event.");

            _labelForDestoryField = new GUIContent("Don't destroy on load",
                "Whether task runners should be destroyed when scene is unloaded.");
        }

        [SettingsProvider]
        private static SettingsProvider CreateProvider()
        {
            return new AsyncSettingsProvider($"{nameof(UnityUtility)}/{TaskSystem.SYSTEM_NAME}",
                                             SettingsScope.Project,
                                             new[] { "Async", "System", "Stop", "Allow", "Task" });
        }

        public override void OnGUI(string searchContext)
        {
            if (_serializedObject == null || _serializedObject.targetObject == null)
            {
                AsyncSystemSettings settings = AssetDatabase.LoadAssetAtPath<AsyncSystemSettings>(_settingsPath);

                if (settings == null)
                {
                    EditorGUILayout.Space();

                    if (GUILayout.Button("Create Settings", GUILayout.MaxWidth(150f), GUILayout.Height(25f)))
                        _serializedObject = new SerializedObject(CreateSettings());
                }
                else
                {
                    _serializedObject = new SerializedObject(settings);
                }

                return;
            }

            _serializedObject.Update();

            SerializedProperty canBeStoppedProperty = _serializedObject.FindProperty(AsyncSystemSettings.CanBeStoppedName);
            SerializedProperty canBeStoppedGloballyProperty = _serializedObject.FindProperty(AsyncSystemSettings.CanBeStoppedGloballyName);

            canBeStoppedProperty.boolValue = EditorGUILayout.Toggle(_labelForStopField, canBeStoppedProperty.boolValue);

            GUI.enabled = canBeStoppedProperty.boolValue;

            canBeStoppedGloballyProperty.boolValue = EditorGUILayout.Toggle(_labelForGlobalStopField, canBeStoppedGloballyProperty.boolValue);

            if (canBeStoppedGloballyProperty.boolValue)
            {
                EditorGUILayout.HelpBox("Are you sure? Stopping all tasks globally is a dangerous practice.", MessageType.Warning);
            }

            GUI.enabled = true;

            _serializedObject.ApplyModifiedProperties();
            canBeStoppedProperty.Dispose();
            canBeStoppedGloballyProperty.Dispose();
        }

        private AsyncSystemSettings CreateSettings()
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, nameof(Resources)));
            AsyncSystemSettings settings = ScriptableObject.CreateInstance<AsyncSystemSettings>();
            AssetDatabase.CreateAsset(settings, _settingsPath);
            AssetDatabase.SaveAssets();
            return settings;
        }
    }
}
#endif
