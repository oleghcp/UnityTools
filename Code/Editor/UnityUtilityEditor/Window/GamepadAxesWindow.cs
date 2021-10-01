#if !UNITY_2018_3_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityUtilityEditor.Window.GamepadAxes;

namespace UnityUtilityEditor.Window
{
    internal class GamepadAxesWindow : EditorWindow
    {
        private GamepadAxesDrawer _drawer;

        public static void Create()
        {
            GamepadAxesWindow window = GetWindow<GamepadAxesWindow>(true, "Gamepad Axes");
            window.minSize = new Vector2(300f, 500f);
        }

        private void OnEnable()
        {
            _drawer = new GamepadAxesDrawer();
        }

        private void OnGUI()
        {
            _drawer.OnGUI();
        }
    }
}
#endif
