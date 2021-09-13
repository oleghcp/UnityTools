using UnityEditor;
using UnityEngine;
using UnityUtility.NodeBased;

namespace UnityUtilityEditor.Inspectors.NodeBased
{
    [CustomEditor(typeof(RawNode), true)]
    internal class NodeEditor : Editor<RawNode>
    {
        private const string DIALOG_TITLE = "Delete node?";
        private const string DIALOG_TEXT = "Are you sure you want delete this node?";
        private const string DELETE_ITEM_NAME = "Delete Node";

        public override void OnInspectorGUI()
        {

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();

                if (GUILayout.Button(DELETE_ITEM_NAME, GUILayout.Width(100f), GUILayout.Height(30f)) &&
                    EditorUtility.DisplayDialog(DIALOG_TITLE, DIALOG_TEXT, "Yes", "No"))
                    DestroyNode(target);

                EditorGUILayout.Space();
            }
        }

        [MenuItem(MenuItems.CONTEXT_MENU_NAME + nameof(RawNode) + "/" + MenuItems.RESET_ITEM_NAME)]
        private static void ResetMenuItem() { }

        [MenuItem(MenuItems.CONTEXT_MENU_NAME + nameof(RawNode) + "/" + MenuItems.RESET_ITEM_NAME, true)]
        private static bool ResetMenuItemValidate() => false;

        [MenuItem(MenuItems.CONTEXT_MENU_NAME + nameof(RawNode) + "/" + DELETE_ITEM_NAME)]
        private static void DeleteMenuItem(MenuCommand command)
        {
            if (EditorUtility.DisplayDialog(DIALOG_TITLE, DIALOG_TEXT, "Yes", "No"))
                DestroyNode(command.context as RawNode);
        }

        private static void DestroyNode(RawNode target)
        {
            RawGraph graph = target.Owner;

            if (graph != null)
            {
                ArrayUtility.Remove(ref graph.Nodes, target);
                if (graph.Nodes.Length == 0)
                    graph.LastId = 0;
                EditorUtility.SetDirty(graph);

                foreach (var node in target.Owner.Nodes)
                {
                    SerializedObject serialized = new SerializedObject(node);
                    SerializedProperty arrayProp = serialized.FindProperty(RawNode.ArrayFieldName);

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
