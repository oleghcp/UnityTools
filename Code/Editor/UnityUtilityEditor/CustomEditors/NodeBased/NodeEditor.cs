using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtility.NodeBased;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor.CustomEditors.NodeBased
{
    [CustomEditor(typeof(Node), true)]
    internal class NodeEditor : Editor<Node>
    {
        public override void OnInspectorGUI()
        {
            const string title = "Delete node?";
            const string text = "Are you sure you want delete this node?";

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();

                if (GUILayout.Button("Delete Node", GUILayout.Width(100f), GUILayout.Height(30f)) &&
                    EditorUtility.DisplayDialog(title, text, "Yes", "No"))
                    DestroyNode();

                EditorGUILayout.Space();
            }
        }

        private void DestroyNode()
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
