using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.SettingsProviders
{
    internal class ProjectBaseSettingsProvider : SettingsProvider
    {
        public ProjectBaseSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        private static SettingsProvider CreateProvider()
        {
            return new ProjectBaseSettingsProvider($"{SettingsProviderUtility.PROJECT_SECTION_NAME}/{LibConstants.LIB_NAME}", SettingsScope.Project);
        }

        public override void OnGUI(string searchContext)
        {
            GUILayout.Label($"{LibConstants.LIB_NAME} Settings");
        }
    }
}
