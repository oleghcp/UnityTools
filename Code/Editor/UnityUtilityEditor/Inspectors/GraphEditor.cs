#if UNITY_2019_3_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityUtility;
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
            GUI.color = Colours.Lime;
            bool openPressed = EditorGuiLayout.CenterButton("Open Graph", GUILayout.Height(40f), GUILayout.Width(200f));
            GUI.color = Colours.White;

            EditorGUILayout.Space();

            bool editPressed = EditorGuiLayout.CenterButton("Edit Script", GUILayout.Height(30f), GUILayout.Width(150f));

            if (openPressed)
                GraphEditorWindow.Open(target);

            if (editPressed)
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
