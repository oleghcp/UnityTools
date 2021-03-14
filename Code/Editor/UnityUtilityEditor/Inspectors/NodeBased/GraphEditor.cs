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
        public override void OnInspectorGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();

                GUI.color = Colours.Lime;
                bool buttonPressed = GUILayout.Button("Open Graph Editor", GUILayout.Height(30f));
                GUI.color = Colours.White;

                if (buttonPressed)
                    GraphEditorWindow.OpenWindow(target as Graph);

                EditorGUILayout.Space();
            }
        }
    }
}
#endif
