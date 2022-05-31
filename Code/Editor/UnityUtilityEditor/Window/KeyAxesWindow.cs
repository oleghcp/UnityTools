#if !UNITY_2019_3_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER
using System;
using UnityEditor;
using UnityEngine;
using UnityUtility.Controls;

namespace UnityUtilityEditor.Window
{
    internal class KeyAxesWindow : EditorWindow
    {
        private SerializedObject _serializedObject;

        private SerializedProperty[] _directs;
        private string[] _names;

        public static void Create(SerializedObject param, Type keyFuncsEnum)
        {
            KeyAxesWindow window = CreateInstance<KeyAxesWindow>();
            window.SetUp(param, keyFuncsEnum);
            Rect buttonRect = new Rect(Event.current.mousePosition, default);
#if UNITY_2019_1_OR_NEWER
            buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
#else
            buttonRect.position = GUIUtility.GUIToScreenPoint(buttonRect.position);
#endif
            window.ShowAsDropDown(buttonRect, new Vector2(200f, getHeigth()));

            float getHeigth()
            {
                const int linesCount = 4;
                float lines = EditorGUIUtility.singleLineHeight * linesCount;
                float spaces = EditorGUIUtility.standardVerticalSpacing * (linesCount + 2);
                return lines + spaces;
            }
        }

        private void SetUp(SerializedObject param, Type keyFuncsEnum)
        {
            _serializedObject = param;

            SerializedProperty prop = _serializedObject.FindProperty(LayoutConfig.KeyAxesFieldName);
            _directs = new[]
            {
                prop.FindPropertyRelative("Up"),
                prop.FindPropertyRelative("Down"),
                prop.FindPropertyRelative("Left"),
                prop.FindPropertyRelative("Right")
            };

            _names = Enum.GetNames(keyFuncsEnum);
        }

        private void OnGUI()
        {
            if (_serializedObject.Disposed())
            {
                Close();
                return;
            }

            GUILayout.BeginArea(new Rect(default, position.size), EditorStyles.helpBox);
            for (int i = 0; i < _directs.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_directs[i].displayName, GUILayout.MaxWidth(75f));
                _directs[i].intValue = EditorGUILayout.Popup(_directs[i].intValue, _names);
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndArea();

            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
