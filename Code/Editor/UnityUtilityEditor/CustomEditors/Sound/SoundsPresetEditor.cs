using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.CustomEditors.Sound
{
    internal abstract class SoundsPresetEditor : Editor
    {
        private SerializedProperty m_nodes;

        private Vector2 m_scrollPos;

        private void OnEnable()
        {
            m_nodes = serializedObject.FindProperty("m_nodes");
        }

        public override void OnInspectorGUI()
        {
            int length = m_nodes.arraySize;

            using (var scope = new EditorGUILayout.ScrollViewScope(m_scrollPos, false, false, GUILayout.MaxHeight((length + 1) * 23f + 5f)))
            {
                DrawTableHeader();

                GUILayout.Space(5f);

                for (int i = 0; i < length; i++)
                {
                    if (DrawTableRow(m_nodes, i))
                        break;

                    GUILayout.Space(3f);
                }

                m_scrollPos = scope.scrollPosition;
            }

            GUILayout.Space(5f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add", GUILayout.MinWidth(100f)))
                    AddObject(m_nodes, null);

                GUILayout.Space(10f);

                if (GUILayout.Button("Sort", GUILayout.MinWidth(100f)))
                    f_sort();

                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(5f);

            UnityObject[] objects = GUIExt.DropArea("Drag and drop your audio clips here.", 50f);

            if (objects.HasAnyData())
            {
                f_addObjects(objects);

                f_sort();

                serializedObject.ApplyModifiedProperties();

                return;
            }

            GUILayout.Space(5f);

            if (m_nodes.arraySize > 0)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    const string description = "Are you sure you want to clear List?";
                    if (GUILayout.Button("Clear List", GUILayout.MinWidth(100f)) &&
                        EditorUtility.DisplayDialog("Clear List?", description, "Yes", "No"))
                        m_nodes.ClearArray();
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

        private void f_addObjects(UnityObject[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] is AudioClip)
                    AddObject(m_nodes, objects[i]);
            }
        }

        private void f_sort()
        {
            int i = 0;
            while (i < m_nodes.arraySize)
            {
                SerializedProperty name = m_nodes.GetArrayElementAtIndex(i).FindPropertyRelative("Name");

                if (name.stringValue.HasAnyData())
                    i++;
                else
                    m_nodes.DeleteArrayElementAtIndex(i);
            }

            m_nodes.SortArray(prop => prop.FindPropertyRelative("Name").stringValue);
        }
    }
}
