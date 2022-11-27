using UnityEditor;
using UnityEngine;
using UnityUtility.CSharp;
using UnityUtility.CSharp.Collections;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Inspectors.Sound
{
    internal abstract class AudioPresetEditor : Editor
    {
        private Vector2 _scrollPos;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty nodesProp = serializedObject.FindProperty("_nodes");
            int length = nodesProp.arraySize;

            using (var scope = new EditorGUILayout.ScrollViewScope(_scrollPos, false, false, GUILayout.MaxHeight((length + 1) * 23f + 5f)))
            {
                DrawTableHeader();

                GUILayout.Space(5f);

                for (int i = 0; i < length; i++)
                {
                    if (DrawTableRow(nodesProp, i))
                        break;

                    GUILayout.Space(3f);
                }

                _scrollPos = scope.scrollPosition;
            }

            GUILayout.Space(5f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add", GUILayout.MinWidth(100f)))
                    AddObject(nodesProp, null);

                GUILayout.Space(10f);

                if (GUILayout.Button("Sort", GUILayout.MinWidth(100f)))
                    Sort(nodesProp);

                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(5f);

            UnityObject[] objects = EditorGuiLayout.DropArea("Drag and drop your audio clips to here.", GUILayout.Height(50f));

            if (objects.HasAnyData())
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] is AudioClip audioClip)
                        AddObject(nodesProp, audioClip);
                }
                Sort(nodesProp);
            }

            GUILayout.Space(5f);

            if (nodesProp.arraySize > 0)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    const string description = "Are you sure you want to clear List?";
                    if (GUILayout.Button("Clear List", GUILayout.MinWidth(100f)) &&
                        EditorUtility.DisplayDialog("Clear List?", description, "Yes", "No"))
                        nodesProp.ClearArray();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        // -- //

        protected abstract void DrawTableHeader();
        protected abstract bool DrawTableRow(SerializedProperty nodes, int index);
        protected abstract void AddObject(SerializedProperty nodes, UnityObject obj);

        // -- //

        private void Sort(SerializedProperty nodesArrayProp)
        {
            int i = 0;
            while (i < nodesArrayProp.arraySize)
            {
                SerializedProperty name = nodesArrayProp.GetArrayElementAtIndex(i).FindPropertyRelative("Name");

                if (name.stringValue.HasAnyData())
                    i++;
                else
                    nodesArrayProp.DeleteArrayElementAtIndex(i);
            }

            nodesArrayProp.SortArray(prop => prop.FindPropertyRelative("Name").stringValue);
        }
    }
}
