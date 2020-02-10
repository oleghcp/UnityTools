using UnityObject = UnityEngine.Object;
using System;
using UnityEngine;
using UnityEditor;
using UU.MathExt;
using UU.Sound;

namespace UUEditor.SoundEditors
{
    internal abstract class SoundsPresetEditor : Editor
    {
        private SerializedProperty m_nodes;

        private Vector2 m_scrollPos;
        private bool m_sure;

        private void Awake()
        {
            m_nodes = serializedObject.FindProperty("m_nodes");
        }

        public override void OnInspectorGUI()
        {
            int length = m_nodes.arraySize;

            m_scrollPos = GUILayout.BeginScrollView(m_scrollPos, false, false, GUILayout.MinHeight(10f), GUILayout.MaxHeight((length + 1) * 23f));
            {
                if (length > 0)
                {
                    DrawTableHeader();
                }

                GUILayout.Space(5f);

                for (int i = 0; i < length; i++)
                {
                    if (DrawTableRow(m_nodes, i))
                        break;

                    GUILayout.Space(3f);
                }
            }
            GUILayout.EndScrollView();

            GUILayout.Space(5f);

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add"))
                    AddObject(m_nodes, null);

                GUILayout.Space(5f);

                if (GUILayout.Button("Sort"))
                    f_sort();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);

            var objects = EditorScriptUtility.DrawDropArea("Drag and drop your audio clips here.", 50f);

            if (objects != null)
            {
                f_addObjects(objects);

                f_sort();

                serializedObject.ApplyModifiedProperties();

                return;
            }

            GUILayout.Space(5f);

            if (m_nodes.arraySize > 0)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Clear List") && m_sure)
                {
                    m_sure = false;
                    m_nodes.ClearArray();
                }
                EditorGUILayout.LabelField("I'm sure:", GUILayout.MaxWidth(55f));
                m_sure = EditorGUILayout.Toggle(m_sure, GUILayout.MaxWidth(20f));
                EditorGUILayout.EndHorizontal();
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
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

            //i = 0;
            //while (i < m_nodes.arraySize - 1)
            //{
            //    SerializedProperty a = m_nodes.GetArrayElementAtIndex(i).FindPropertyRelative("Name");
            //    SerializedProperty b = m_nodes.GetArrayElementAtIndex(i + 1).FindPropertyRelative("Name");

            //    if (a.stringValue == b.stringValue)
            //        m_nodes.DeleteArrayElementAtIndex(i + 1);
            //    else
            //        i++;
            //}
        }
    }
}
