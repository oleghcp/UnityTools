using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.CustomEditors.Sound
{
    internal abstract class SoundsPresetEditor : Editor
    {
        private SerializedProperty _nodes;

        private Vector2 _scrollPos;

        private void OnEnable()
        {
            _nodes = serializedObject.FindProperty("_nodes");
        }

        public override void OnInspectorGUI()
        {
            int length = _nodes.arraySize;

            using (var scope = new EditorGUILayout.ScrollViewScope(_scrollPos, false, false, GUILayout.MaxHeight((length + 1) * 23f + 5f)))
            {
                DrawTableHeader();

                GUILayout.Space(5f);

                for (int i = 0; i < length; i++)
                {
                    if (DrawTableRow(_nodes, i))
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
                    AddObject(_nodes, null);

                GUILayout.Space(10f);

                if (GUILayout.Button("Sort", GUILayout.MinWidth(100f)))
                    Sort();

                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(5f);

            UnityObject[] objects = GUIExt.DropArea("Drag and drop your audio clips here.", 50f);

            if (objects.HasAnyData())
            {
                AddObjects(objects);

                Sort();

                serializedObject.ApplyModifiedProperties();

                return;
            }

            GUILayout.Space(5f);

            if (_nodes.arraySize > 0)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    const string description = "Are you sure you want to clear List?";
                    if (GUILayout.Button("Clear List", GUILayout.MinWidth(100f)) &&
                        EditorUtility.DisplayDialog("Clear List?", description, "Yes", "No"))
                        _nodes.ClearArray();
                }
            }

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }

        // -- //

        protected abstract void DrawTableHeader();
        protected abstract bool DrawTableRow(SerializedProperty nodes, int index);
        protected abstract void AddObject(SerializedProperty nodes, UnityObject obj);

        // -- //

        private void AddObjects(UnityObject[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] is AudioClip)
                    AddObject(_nodes, objects[i]);
            }
        }

        private void Sort()
        {
            int i = 0;
            while (i < _nodes.arraySize)
            {
                SerializedProperty name = _nodes.GetArrayElementAtIndex(i).FindPropertyRelative("Name");

                if (name.stringValue.HasAnyData())
                    i++;
                else
                    _nodes.DeleteArrayElementAtIndex(i);
            }

            _nodes.SortArray(prop => prop.FindPropertyRelative("Name").stringValue);
        }
    }
}
