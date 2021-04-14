using UnityEditor;
using UnityEngine;
using UnityUtility.NodeBased;
using UnityUtilityEditor.NodeBased;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor.Inspectors.NodeBased
{
    [CustomEditor(typeof(RawNode), true)]
    internal class NodeEditor : Editor<RawNode>
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

        [MenuItem("CONTEXT/" + nameof(RawNode) + "/" + MENU_NAME)]
        private static void MenuItem(MenuCommand command)
        {
            if (EditorUtility.DisplayDialog(DIALOG_TITLE, DIALOG_TEXT, "Yes", "No"))
                DestroyNode(command.context as RawNode);
        }

        private static void DestroyNode(RawNode target)
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

                foreach (var node in target.Owner.Nodes)
                {
                    SerializedObject serialized = new SerializedObject(node);
                    SerializedProperty arrayProp = serialized.FindProperty(DummyNode.ArrayFieldName);

                    int index = arrayProp.GetArrayElement(out var connected, predicate);

                    if (index >= 0)
                    {
                        arrayProp.DeleteArrayElementAtIndex(index);
                        serialized.ApplyModifiedPropertiesWithoutUndo();
                    }
                }

                EditorUtilityExt.SaveProject();
            }

            DestroyImmediate(target, true);
            AssetDatabase.SaveAssets();

            bool predicate(SerializedProperty property)
            {
                SerializedProperty nodeProp = property.FindPropertyRelative(Transition.NodeFieldName);
                return nodeProp.objectReferenceValue == target;
            }
        }
    }
}
#endif
