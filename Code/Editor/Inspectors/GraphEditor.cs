﻿using OlegHcp;
using OlegHcp.NodeBased.Service;
using OlegHcpEditor.MenuItems;
using OlegHcpEditor.Window;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Inspectors
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

        private const string resetMenuItemName = MenuItemsUtility.CONTEXT_MENU_NAME + nameof(RawGraph) + "/" + MenuItemsUtility.RESET_ITEM_NAME;

        [MenuItem(resetMenuItemName)]
        private static void ResetMenuItem() { }

        [MenuItem(resetMenuItemName, true)]
        private static bool ResetMenuItemValidate() => false;

        [MenuItem(MenuItemsUtility.CONTEXT_MENU_NAME + nameof(RawGraph) + "/" + OPEN_ITEM_NAME)]
        private static void OpenMenuItem(MenuCommand command)
        {
            GraphEditorWindow.Open(command.context as RawGraph);
        }
    }
}
