using System;
using UnityEngine;
using UnityUtility.MathExt;
using UnityEditor;
using UnityUtility.Collections;
using UnityUtilityEditor.Drawers;
using UnityUtility.BitMasks;

namespace UnityUtilityEditor.Window
{
    internal class SimpleBitArrayMaskWindow : BitArrayMaskWindow
    {
        private SerializedObject m_serializedObject;
        private SerializedProperty m_length;
        private SerializedProperty m_array;

        private Vector2 m_scrollPos;

        public override void SetUp(object param)
        {
            SerializedProperty property = param as SerializedProperty;
            m_serializedObject = property.serializedObject;
            m_length = property.FindPropertyRelative(BitArrayMask.LengthFieldName);
            m_array = property.FindPropertyRelative(BitArrayMask.ArrayFieldName);
        }

        private void Awake()
        {
            minSize = new Vector2(300f, 300f);
            maxSize = new Vector2(300f, 1500f);
        }

        private void Update()
        {
            if (m_serializedObject.Disposed())
                Close();
        }

        private void OnGUI()
        {
            if (m_serializedObject.Disposed())
            {
                Close();
                return;
            }

            GUILayout.Space(10f);

            m_length.intValue = EditorGUILayout.IntField("Length", m_length.intValue).CutBefore(0);
            f_checkArray();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical();
            m_scrollPos.y = GUILayout.BeginScrollView(m_scrollPos, false, false).y;
            int size = m_array.arraySize;
            int length = m_length.intValue;
            for (int i = 0; i < size; i++)
            {
                int len = length > 32 ? 32 : length;

                var prop = m_array.GetArrayElementAtIndex(i);
                int mask = prop.intValue;

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("All", GUILayout.Width(60f)))
                    mask.AddAll(len);
                if (GUILayout.Button("None", GUILayout.Width(60f)))
                    mask.Clear();

                EditorGUILayout.LabelField(mask.ToString(), GUILayout.Width(120f));
                EditorGUILayout.EndHorizontal();

                for (int j = 0; j < len; j++)
                {
                    mask.SetFlag(j, EditorGUILayout.Toggle((32 * i + j).ToString(), mask.ContainsFlag(j)));
                }
                length -= 32;

                prop.intValue = mask;
            }
            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            if (GUI.changed)
                m_serializedObject.ApplyModifiedProperties();
        }

        private void f_checkArray()
        {
            int len = BitArrayMask.GetArrayLength(m_length.intValue);
            int size = m_array.arraySize;

            if (len > size)
            {
                for (int i = 0; i < len - size; i++)
                {
                    int index = m_array.arraySize;
                    m_array.InsertArrayElementAtIndex(m_array.arraySize);
                    m_array.GetArrayElementAtIndex(index).intValue = 0;
                }
            }
            else if (len < size)
            {
                for (int i = 0; i < size - len; i++)
                {
                    m_array.DeleteArrayElementAtIndex(m_array.arraySize - 1);
                }
            }
        }
    }
}
