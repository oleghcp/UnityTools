using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Collections;
using UnityUtilityEditor.Drawers;

namespace UnityUtilityEditor.Window.BitArrays
{
    internal class EnumBitArrayMaskWindow : BitArrayMaskWindow
    {
        private SerializedObject _serializedObject;
        private SerializedProperty _length;
        private SerializedProperty _array;
        private string[] _names;
        private Vector2 _scrollPos;

        public override void SetUp(object param)
        {
            var data = param as Tuple<SerializedProperty, string[]>;
            var property = data.Item1;
            _serializedObject = property.serializedObject;
            _length = property.FindPropertyRelative(BitArrayMask.LengthFieldName);
            _array = property.FindPropertyRelative(BitArrayMask.ArrayFieldName);
            _names = data.Item2;

            CheckArray(_length, _array, _names);
            _serializedObject.ApplyModifiedProperties();
        }

        private void Awake()
        {
            minSize = new Vector2(300f, 300f);
            maxSize = new Vector2(300f, 1500f);
        }

        private void Update()
        {
            if (_serializedObject.Disposed())
                Close();
        }

        private void OnGUI()
        {
            if (_serializedObject.Disposed())
                return;

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical();
            _scrollPos.y = GUILayout.BeginScrollView(_scrollPos, false, false).y;
            int index = 0;
            SerializedProperty prop = _array.GetArrayElementAtIndex(index);
            int mask = prop.intValue;
            for (int i = 0; i < _names.Length; i++)
            {
                if (_names[i] != null)
                {
                    if (index != i / BitMask.SIZE)
                    {
                        prop.intValue = mask;
                        index = i / BitMask.SIZE;
                        prop = _array.GetArrayElementAtIndex(index);
                        mask = prop.intValue;
                    }
                    bool hasFlag = BitMask.HasFlag(mask, i % BitMask.SIZE);
                    BitMask.SetFlag(ref mask, i % BitMask.SIZE, EditorGUILayout.Toggle(_names[i], hasFlag));
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
                int size = _array.arraySize;
                for (int i = 0; i < size; i++)
                {
                    _array.GetArrayElementAtIndex(i).intValue = -1;
                }
            }
            if (GUILayout.Button("None"))
            {
                int size = _array.arraySize;
                for (int i = 0; i < size; i++)
                {
                    _array.GetArrayElementAtIndex(i).intValue = 0;
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            if (GUI.changed)
                _serializedObject.ApplyModifiedProperties();
        }

        public static void CheckArray(SerializedProperty bitMask, string[] names)
        {
            var length = bitMask.FindPropertyRelative(BitArrayMask.LengthFieldName);
            var array = bitMask.FindPropertyRelative(BitArrayMask.ArrayFieldName);

            CheckArray(length, array, names);
        }

        private static void CheckArray(SerializedProperty length, SerializedProperty array, string[] names)
        {
            if (length.intValue != names.Length)
                length.intValue = names.Length;

            int countedSize = BitArrayMask.GetArraySize(names.Length);
            int realSize = array.arraySize;

            if (countedSize > realSize)
            {
                for (int i = 0; i < countedSize - realSize; i++)
                {
                    array.PlaceArrayElement().intValue = 0;
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
