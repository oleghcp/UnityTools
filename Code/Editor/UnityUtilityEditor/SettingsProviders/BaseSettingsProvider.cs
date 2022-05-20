#if UNITY_2018_3_OR_NEWER
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
            return new BaseSettingsProvider($"{SettingsScope.Project}/{nameof(UnityUtility)}", SettingsScope.Project);
        }

        public override void OnGUI(string searchContext)
        {
            GUILayout.Label($"{nameof(UnityUtility)} Settings");
        }
    }
}
#endif
