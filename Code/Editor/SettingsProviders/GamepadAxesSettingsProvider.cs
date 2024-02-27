#if !UNITY_2019_3_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER
using System.Collections.Generic;
using UnityEditor;
using OlegHcpEditor.Window.GamepadAxes;

namespace OlegHcpEditor.SettingsProviders
{
    internal class GamepadAxesSettingsProvider : SettingsProvider
    {
        private GamepadAxesDrawer _drawer;

        public GamepadAxesSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
            _drawer = new GamepadAxesDrawer();
        }

        [SettingsProvider]
        private static SettingsProvider CreateProvider()
        {
            return new GamepadAxesSettingsProvider($"{SettingsProviderUtility.PROJECT_SECTION_NAME}/{LibConstants.LIB_NAME}/Gamepad Axes",
                                                   SettingsScope.Project,
                                                   new[] { "Gamepad", "Axes" });
        }

        public override void OnGUI(string searchContext)
        {
            _drawer.OnGUI();
        }
    }
}
#endif
