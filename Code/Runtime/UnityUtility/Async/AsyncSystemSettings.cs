using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityUtility.Async
{
    public class AsyncSystemSettings : ScriptableObject, IAsyncSettings
    {
        [SerializeField]
        private bool _canBeStopped = true;
        [SerializeField]
        private bool _canBeStoppedGlobally = false;

        public bool CanBeStopped => _canBeStopped;
        public bool CanBeStoppedGlobally => _canBeStopped && _canBeStoppedGlobally;

#if UNITY_EDITOR
        private static readonly string SETTINGS_PATH = $"Assets/{nameof(Resources)}/{nameof(AsyncSystemSettings)}.asset";

        public static string CanBeStoppedName => nameof(_canBeStopped);
        public static string CanBeStoppedGloballyName => nameof(_canBeStoppedGlobally);

        public static AsyncSystemSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<AsyncSystemSettings>(SETTINGS_PATH);
            if (settings == null)
            {
                string path = Path.Combine(Application.dataPath, nameof(Resources));
                Directory.CreateDirectory(path);
                settings = CreateInstance<AsyncSystemSettings>();
                AssetDatabase.CreateAsset(settings, SETTINGS_PATH);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        public static SerializedObject GetSerializedObject()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
#endif
    }
}
