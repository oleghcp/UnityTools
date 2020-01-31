using System;
using UnityEngine;
using UnityEditor;
using UU.Collections;
using UUEditor.Drawers;

namespace UUEditor.Windows
{
    internal class EnumBitArrayMaskWindow : BitArrayMaskWindow
    {
        private SerializedObject m_serializedObject;
        private SerializedProperty m_length;
        private SerializedProperty m_array;
        private string[] m_names;
        private Vector2 m_scrollPos;

        public override void SetUp(object param)
        {
            var data = param as Tuple<SerializedProperty, string[]>;
            var property = data.Item1;
            m_serializedObject = property.serializedObject;
            m_length = property.FindPropertyRelative(BitArrayMask.LengthFieldName);
            m_array = property.FindPropertyRelative(BitArrayMask.ArrayFieldName);
            m_names = data.Item2;

            f_checkArray(m_length, m_array, m_names);
            m_serializedObject.ApplyModifiedProperties();
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
                return;

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical();
            m_scrollPos.y = GUILayout.BeginScrollView(m_scrollPos, false, false).y;
            int index = 0;
            SerializedProperty prop = m_array.GetArrayElementAtIndex(index);
            int mask = prop.intValue;
            for (int i = 0; i < m_names.Length; i++)
            {
                if (m_names[i] != null)
                {
                    if (index != i / 32)
                    {
                        prop.intValue = mask;
                        index = i / 32;
                        prop = m_array.GetArrayElementAtIndex(index);
                        mask = prop.intValue;
                    }
                    mask.SetFlag(i % 32, EditorGUILayout.Toggle(m_names[i], mask.ContainsFlag(i % 32)));
                }
            }
            prop.intValue = mask;
            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("All"))
            {
                int size = m_array.arraySize;
                for (int i = 0; i < size; i++)
                {
                    m_array.GetArrayElementAtIndex(i).intValue = -1;
                }
            }
            if (GUILayout.Button("None"))
            {
                int size = m_array.arraySize;
                for (int i = 0; i < size; i++)
                {
                    m_array.GetArrayElementAtIndex(i).intValue = 0;
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            if (GUI.changed)
                m_serializedObject.ApplyModifiedProperties();
        }

        public static void CheckArray(SerializedProperty bitMask, string[] names)
        {
            var length = bitMask.FindPropertyRelative(BitArrayMask.LengthFieldName);
            var array = bitMask.FindPropertyRelative(BitArrayMask.ArrayFieldName);

            f_checkArray(length, array, names);
        }

        private static void f_checkArray(SerializedProperty length, SerializedProperty array, string[] names)
        {
            if (length.intValue != names.Length)
                length.intValue = names.Length;

            int countedSize = BitArrayMask.GetArrayLength(names.Length, 32);
            int realSize = array.arraySize;

            if (countedSize > realSize)
            {
                for (int i = 0; i < countedSize - realSize; i++)
                {
                    int index = array.arraySize;
                    array.InsertArrayElementAtIndex(array.arraySize);
                    array.GetArrayElementAtIndex(index).intValue = 0;
                }
            }
            else if (countedSize < realSize)
            {
                for (int i = 0; i < realSize - countedSize; i++)
                {
                    array.DeleteArrayElementAtIndex(array.arraySize - 1);
                }
            }
        }
    }
}
