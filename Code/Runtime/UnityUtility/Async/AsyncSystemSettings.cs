using UnityEditor;
using UnityEngine;

namespace UnityUtility.Async
{
    public class AsyncSystemSettings : ScriptableObject, IAsyncSettings
    {
        [SerializeField]
        private bool _canBeStopped = true;
        [SerializeField]
        private bool _canBeStoppedGlobaly;
        [SerializeField]
        private bool _dontDestroyOnLoad = true;

        public bool CanBeStopped => _canBeStopped;
        public bool CanBeStoppedGlobaly => _canBeStopped && _canBeStoppedGlobaly;
        public bool DoNotDestroyOnLoad => !_canBeStopped || _dontDestroyOnLoad;

#if UNITY_EDITOR
        private static readonly string SETTINGS_PATH = $"Assets/Resources/{nameof(AsyncSystemSettings)}.asset";

        private static string CanBeStoppedName => nameof(_canBeStopped);
        private static string CanBeStoppedGlobalyName => nameof(_canBeStoppedGlobaly);
        private static string DontDestroyOnLoadName => nameof(_dontDestroyOnLoad);

        public static AsyncSystemSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<AsyncSystemSettings>(SETTINGS_PATH);
            if (settings == null)
            {
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
