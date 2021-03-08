using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Collections;
using UnityUtility.MathExt;
using UnityUtilityEditor.Drawers;

namespace UnityUtilityEditor.Window.BitArrays
{
    internal class SimpleBitArrayMaskWindow : BitArrayMaskWindow
    {
        private SerializedObject _serializedObject;
        private SerializedProperty _length;
        private SerializedProperty _array;

        private Vector2 _scrollPos;

        public override void SetUp(object param)
        {
            SerializedProperty property = param as SerializedProperty;
            _serializedObject = property.serializedObject;
            _length = property.FindPropertyRelative(BitArrayMask.LengthFieldName);
            _array = property.FindPropertyRelative(BitArrayMask.ArrayFieldName);
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
            {
                Close();
                return;
            }

            GUILayout.Space(10f);

            _length.intValue = EditorGUILayout.IntField("Length", _length.intValue).CutBefore(0);
            CheckArray();

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical();
            _scrollPos.y = GUILayout.BeginScrollView(_scrollPos, false, false).y;
            int size = _array.arraySize;
            int length = _length.intValue;
            for (int i = 0; i < size; i++)
            {
                int len = length > BitMask.SIZE ? BitMask.SIZE : length;

                var prop = _array.GetArrayElementAtIndex(i);
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
                _serializedObject.ApplyModifiedProperties();
        }

        private void CheckArray()
        {
            int len = BitArrayMask.GetArraySize(_length.intValue);
            int size = _array.arraySize;

            if (len > size)
            {
                for (int i = 0; i < len - size; i++)
                {
                    _array.PlaceArrayElement()
                           .intValue = 0;
                }
            }
            else if (len < size)
            {
                for (int i = 0; i < size - len; i++)
                {
                    _array.DeleteArrayElementAtIndex(_array.arraySize - 1);
                }
            }
        }
    }
}
