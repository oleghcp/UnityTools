using System;
using System.IO;
using UnityEngine;
using UnityUtility.Mathematics;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Configs
{
    [Serializable]
    internal class LibrarySettings
    {
        private const string FILE_PATH = AssetDatabaseExt.USER_SETTINGS_FOLDER + LibConstants.LIB_NAME + "Settings.json";

        [SerializeField]
        private bool _openFoldersByDoubleClick = true;
        [SerializeField]
        private bool _openScriptableAssetsAsCode = true;
        [SerializeField]
        private string _suppressedWarningsInIde = "CS0649";
        [SerializeField]
        private int _namespaceFolderRootSkipSteps;

        private static LibrarySettings _instance;

        public static bool OpenFoldersByDoubleClick
        {
            get => Get()._openFoldersByDoubleClick;
            set => SetField(ref Get()._openFoldersByDoubleClick, value);
        }

        public static bool OpenScriptableAssetsAsCode
        {
            get => Get()._openScriptableAssetsAsCode;
            set => SetField(ref Get()._openScriptableAssetsAsCode, value);
        }

        public static string SuppressedWarningsInIde
        {
            get => Get()._suppressedWarningsInIde;
            set => SetField(ref Get()._suppressedWarningsInIde, value);
        }

        public static int NamespaceFolderRootSkipSteps
        {
            get => Get()._namespaceFolderRootSkipSteps.ClampMin(0);
            set => SetField(ref Get()._namespaceFolderRootSkipSteps, value);
        }

        private static LibrarySettings Get()
        {
            return _instance ?? (_instance = Load());
        }

        private static void Save(LibrarySettings instance)
        {
            if (!Directory.Exists(AssetDatabaseExt.USER_SETTINGS_FOLDER))
                Directory.CreateDirectory(AssetDatabaseExt.USER_SETTINGS_FOLDER);

            string json = JsonUtility.ToJson(instance, true);
            File.WriteAllText(FILE_PATH, json);
        }

        private static LibrarySettings Load()
        {
            if (!File.Exists(FILE_PATH))
            {
                var instance = new LibrarySettings();
                Save(instance);
                return instance;
            }
            else
            {
                string json = File.ReadAllText(FILE_PATH);
                return JsonUtility.FromJson<LibrarySettings>(json);
            }
        }

        private static void SetField<T>(ref T field, T value) where T : IEquatable<T>
        {
            if (field.Equals(value))
                return;

            field = value;
            Save(Get());
        }
    }
}
