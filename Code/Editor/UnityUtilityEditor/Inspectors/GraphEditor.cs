#if UNITY_2019_3_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityUtility.NodeBased;
using UnityUtilityEditor.Window.NodeBased;

namespace UnityUtilityEditor.Inspectors
{
    [CustomEditor(typeof(RawGraph), true)]
    internal class GraphEditor : Editor<RawGraph>
    {
        private const string OPEN_ITEM_NAME = "Open Graph Editor";

        public override void OnInspectorGUI()
        {
            bool buttonPressed = false;

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();

                buttonPressed = GUILayout.Button("Edit Script", GUILayout.Height(30f));

                EditorGUILayout.Space();
            }

            if (buttonPressed)
                EditorUtilityExt.OpenScriptableObjectCode(target);
        }

        [MenuItem(MenuItems.CONTEXT_MENU_NAME + nameof(RawGraph) + "/" + MenuItems.RESET_ITEM_NAME)]
        private static void ResetMenuItem() { }

        [MenuItem(MenuItems.CONTEXT_MENU_NAME + nameof(RawGraph) + "/" + MenuItems.RESET_ITEM_NAME, true)]
        private static bool ResetMenuItemValidate() => false;

        [MenuItem(MenuItems.CONTEXT_MENU_NAME + nameof(RawGraph) + "/" + OPEN_ITEM_NAME)]
        private static void OpenMenuItem(MenuCommand command)
        {
            GraphEditorWindow.Open(command.context as RawGraph);
        }
    }
}
#endif
