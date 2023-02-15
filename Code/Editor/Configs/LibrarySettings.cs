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
        [SerializeField]
        private int _namespaceFolderRootSkipSteps;

        private static LibrarySettings _instance;

        public static int NamespaceFolderRootSkipSteps => Get()._namespaceFolderRootSkipSteps;

        public static void SetNamespaceFolderRootSkipSteps(int steps)
        {
            if (steps < 0)
                throw Errors.NegativeParameter(nameof(steps));

            LibrarySettings instance = Get();

            instance._namespaceFolderRootSkipSteps = steps;
            Save(instance);
        }

        private static LibrarySettings Get()
        {
            return _instance ?? (_instance = Load());
        }

        private static void Save(LibrarySettings instance)
        {
            File.WriteAllText(GetPath(), JsonUtility.ToJson(instance, true));
        }

        private static LibrarySettings Load()
        {
            string settingsPath = GetPath();

            if (!File.Exists(settingsPath))
            {
                var instance = new LibrarySettings();
                Save(instance);
                return instance;
            }
            else
            {
                string json = File.ReadAllText(settingsPath);
                return JsonUtility.FromJson<LibrarySettings>(json);
            }
        }

        private static string GetPath()
        {
            return $"{AssetDatabaseExt.PROJECT_SETTINGS_FOLDER}{nameof(UnityUtility)}Settings.json";
        }
    }
}
