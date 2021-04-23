using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.NodeBased;
using UnityUtilityEditor.Window.NodeBased;

namespace UnityUtilityEditor.Inspectors.NodeBased
{
    [CustomEditor(typeof(RawGraph), true)]
    internal class GraphEditor : Editor<RawGraph>
    {
        private const string OPEN_ITEM_NAME = "Open Graph Editor";

        public override void OnInspectorGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();

                GUI.color = Colours.Lime;
                bool buttonPressed = GUILayout.Button(OPEN_ITEM_NAME, GUILayout.Height(30f));
                GUI.color = Colours.White;

                if (buttonPressed)
                    GraphEditorWindow.OpenWindow(target);

                EditorGUILayout.Space();
            }
        }

        [MenuItem(MenuItems.CONTEXT_MENU_NAME + nameof(RawGraph) + "/" + MenuItems.RESET_ITEM_NAME)]
        private static void ResetMenuItem() { }

        [MenuItem(MenuItems.CONTEXT_MENU_NAME + nameof(RawGraph) + "/" + MenuItems.RESET_ITEM_NAME, true)]
        private static bool ResetMenuItemValidate() => false;

        [MenuItem(MenuItems.CONTEXT_MENU_NAME + nameof(RawGraph) + "/" + OPEN_ITEM_NAME)]
        private static void OpenMenuItem(MenuCommand command)
        {
            GraphEditorWindow.OpenWindow(command.context as RawGraph);
        }
    }
}
