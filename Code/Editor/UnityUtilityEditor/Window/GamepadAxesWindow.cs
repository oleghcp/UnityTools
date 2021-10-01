using UnityEditor;
using UnityEngine;
using UnityUtilityEditor.SettingsProviders;

namespace UnityUtilityEditor.Window
{
    internal class GamepadAxesWindow : EditorWindow
    {
        private SettingsProvider _settingsPropvider;

        public static void Create()
        {
            GamepadAxesWindow window = GetWindow<GamepadAxesWindow>(true, "Gamepad Axes");
            window.minSize = new Vector2(300f, 500f);
        }

        private void OnEnable()
        {
            _settingsPropvider = GamepadAxesSettingsProvider.CreateProvider();
        }

        private void OnGUI()
        {
            _settingsPropvider.OnGUI(null);
        }
    }
}
