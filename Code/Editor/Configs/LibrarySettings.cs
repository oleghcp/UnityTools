using System;
using System.IO;
using UnityEngine;
using UnityUtility.Tools;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Configs
{
    [Serializable]
    internal class LibrarySettings
    {
        private static LibrarySettings _instance;

        [SerializeField]
        private int _namespaceFolderRootSkipSteps;

        public static LibrarySettings I => _instance ?? (_instance = GetSetting());
        public int NamespaceFolderRootSkipSteps => _namespaceFolderRootSkipSteps;

        public void SetNamespaceFolderRootSkipSteps(int steps)
        {
            if (steps < 0)
                throw Errors.NegativeParameter(nameof(steps));

            _namespaceFolderRootSkipSteps = steps;
            File.WriteAllText(GetPath(), JsonUtility.ToJson(this, true));
        }

        private static LibrarySettings GetSetting()
        {
            string settingsPath = GetPath();

            if (!File.Exists(settingsPath))
            {
                File.Create(settingsPath);
                return new LibrarySettings();
            }

            string json = File.ReadAllText(settingsPath);
            return JsonUtility.FromJson<LibrarySettings>(json);
        }

        private static string GetPath()
        {
            return $"{AssetDatabaseExt.PROJECT_SETTINGS_FOLDER}{nameof(UnityUtility)}Settings.json";
        }
    }
}
