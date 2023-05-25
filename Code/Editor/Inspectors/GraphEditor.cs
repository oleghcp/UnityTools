using MenuItems;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.NodeBased.Service;
using UnityUtilityEditor.Engine;
using UnityUtilityEditor.MenuItems;
using UnityUtilityEditor.Window;

namespace UnityUtilityEditor.Inspectors
{
    [CustomEditor(typeof(RawGraph), true)]
    internal class GraphEditor : Editor<RawGraph>
    {
        private const string OPEN_ITEM_NAME = "Open Graph Editor";

        public override void OnInspectorGUI()
        {
            EditorGuiLayout.BeginHorizontalCentering();
            GUI.color = Colours.Lime;
            bool openPressed = GUILayout.Button("Open Graph", GUILayout.Height(40f), GUILayout.Width(200f));
            GUI.color = Colours.White;
            EditorGuiLayout.EndHorizontalCentering();

            EditorGUILayout.Space();

            EditorGuiLayout.BeginHorizontalCentering();
            bool editPressed = GUILayout.Button("Edit Script", GUILayout.Height(30f), GUILayout.Width(150f));
            EditorGuiLayout.EndHorizontalCentering();

            if (openPressed)
                GraphEditorWindow.Open(target);

            if (editPressed)
                EditorUtilityExt.OpenScriptableObjectCode(target);
        }

        [MenuItem(MenuItemsUtility.CONTEXT_MENU_NAME + nameof(RawGraph) + "/" + MenuItemsUtility.RESET_ITEM_NAME)]
        private static void ResetMenuItem() { }

        [MenuItem(MenuItemsUtility.CONTEXT_MENU_NAME + nameof(RawGraph) + "/" + MenuItemsUtility.RESET_ITEM_NAME, true)]
        private static bool ResetMenuItemValidate() => false;

        [MenuItem(MenuItemsUtility.CONTEXT_MENU_NAME + nameof(RawGraph) + "/" + OPEN_ITEM_NAME)]
        private static void OpenMenuItem(MenuCommand command)
        {
            GraphEditorWindow.Open(command.context as RawGraph);
        }
    }
}
