using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.NodeBased;
using UnityUtilityEditor.Window.NodeBased;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor.Inspectors.NodeBased
{
    [CustomEditor(typeof(Graph), true)]
    internal class GraphEditor : Editor
    {
        private const string MENU_NAME = "Open Graph Editor";

        public override void OnInspectorGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();

                GUI.color = Colours.Lime;
                bool buttonPressed = GUILayout.Button(MENU_NAME, GUILayout.Height(30f));
                GUI.color = Colours.White;

                if (buttonPressed)
                    GraphEditorWindow.OpenWindow(target as Graph);

                EditorGUILayout.Space();
            }
        }

        [MenuItem("CONTEXT/Graph/" + MENU_NAME)]
        private static void MenuItem(MenuCommand command)
        {
            GraphEditorWindow.OpenWindow(command.context as Graph);
        }
    }
}
#endif
