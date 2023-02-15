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
        private const string FILE_PATH = AssetDatabaseExt.USER_SETTINGS_FOLDER + nameof(UnityUtility) + "Settings.json";

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
    }
}
