using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.SettingsProviders
{
    internal class BaseSettingsProvider : SettingsProvider
    {
        public BaseSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        private static SettingsProvider CreateProvider()
        {
            return new BaseSettingsProvider($"{SettingsScope.Project}/{LibConstants.LIB_NAME}", SettingsScope.Project);
        }

        public override void OnGUI(string searchContext)
        {
            GUILayout.Label($"{LibConstants.LIB_NAME} Settings");
        }
    }
}
