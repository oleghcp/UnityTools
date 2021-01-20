using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Collections;
using UnityUtility.MathExt;
using UnityUtilityEditor.Drawers;

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
                int len = length > BitMask.SIZE ? BitMask.SIZE : length;

                var prop = m_array.GetArrayElementAtIndex(i);
                int mask = prop.intValue;

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("All", GUILayout.Width(60f)))
                    BitMask.AddAll(ref mask, len);
                if (GUILayout.Button("None", GUILayout.Width(60f)))
                    BitMask.Clear(ref mask);

                EditorGUILayout.LabelField(mask.ToString(), GUILayout.Width(120f));
                EditorGUILayout.EndHorizontal();

                for (int j = 0; j < len; j++)
                {
                    bool hasFlag = BitMask.HasFlag(mask, j);
                    BitMask.SetFlag(ref mask, j, EditorGUILayout.Toggle((BitMask.SIZE * i + j).ToString(), hasFlag));
                }
                length -= BitMask.SIZE;

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
            int len = BitArrayMask.GetArraySize(m_length.intValue);
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
