using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtility.NodeBased;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor.Inspectors.NodeBased
{
    [CustomEditor(typeof(Node), true)]
    internal class NodeEditor : Editor<Node>
    {
        private const string DIALOG_TITLE = "Delete node?";
        private const string DIALOG_TEXT = "Are you sure you want delete this node?";
        private const string MENU_NAME = "Delete Node";

        public override void OnInspectorGUI()
        {

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();

                if (GUILayout.Button(MENU_NAME, GUILayout.Width(100f), GUILayout.Height(30f)) &&
                    EditorUtility.DisplayDialog(DIALOG_TITLE, DIALOG_TEXT, "Yes", "No"))
                    DestroyNode(target);

                EditorGUILayout.Space();
            }
        }

        [MenuItem("CONTEXT/Node/" + MENU_NAME)]
        private static void MenuItem(MenuCommand command)
        {
            if (EditorUtility.DisplayDialog(DIALOG_TITLE, DIALOG_TEXT, "Yes", "No"))
                DestroyNode(command.context as Node);
        }

        private static void DestroyNode(Node target)
        {
            Graph graph = target.Owner;

            if (graph != null)
            {
                ArrayUtility.Remove(ref graph.Nodes, target);
                if (graph.Nodes.Length == 0)
                {
                    graph.LastId = 0;
                    graph.CameraPosition = default;
                }
                EditorUtility.SetDirty(graph);

                foreach (Node node in target.Owner.Nodes)
                {
                    int index = node.Next.IndexOf(item => item.Node == target);

                    if (index >= 0)
                    {
                        ArrayUtility.RemoveAt(ref node.Next, index);
                        EditorUtility.SetDirty(node);
                    }
                }

                EditorUtilityExt.SaveProject();
            }

            DestroyImmediate(target, true);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
