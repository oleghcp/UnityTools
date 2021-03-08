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

        public static KeyAxesWindow Create()
        {
            return GetWindow<KeyAxesWindow>(true, "Set Key Axes");
        }

        private void Awake()
        {
            minSize = new Vector2(200f, 100f);
            maxSize = new Vector2(200f, 100f);
        }

        public void SetUp(SerializedObject param, Type keyFuncsEnum)
        {
            _serializedObject = param;

            var prop = _serializedObject.FindProperty(LayoutConfig.KeyAxesFieldName);
            _directs = new[]
            {
                prop.FindPropertyRelative("Up"),
                prop.FindPropertyRelative("Down"),
                prop.FindPropertyRelative("Left"),
                prop.FindPropertyRelative("Right")
            };

            _names = Enum.GetNames(keyFuncsEnum);
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

            for (int i = 0; i < _directs.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_directs[i].displayName, GUILayout.MaxWidth(100f));
                _directs[i].intValue = EditorGUILayout.Popup(_directs[i].intValue, _names);
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10f);

            if (GUI.changed)
                _serializedObject.ApplyModifiedProperties();
        }
    }
}
