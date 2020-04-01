using System;
using UnityEngine;
using UnityUtility.MathExt;
using UnityEditor;
using UnityUtility.Collections;
using UnityUtilityEditor.Drawers;

namespace UnityUtilityEditor.Window
{
    internal class KeyAxesWindow : EditorWindow
    {
        private SerializedObject m_serializedObject;

        private SerializedProperty[] m_directs;
        private string[] m_names;

        public static KeyAxesWindow Create()
        {
            return GetWindow<KeyAxesWindow>(true, "Set Key Axes");
        }

        private void Awake()
        {
            minSize = new Vector2(200f, 100f);
            maxSize = new Vector2(200f, 100f);
        }

        public void SetUp(object param, Type keyFuncsEnum)
        {
            m_serializedObject = param as SerializedObject;

            var prop = m_serializedObject.FindProperty("KeyAxes");
            m_directs = new[]
            {
                prop.FindPropertyRelative("Up"),
                prop.FindPropertyRelative("Down"),
                prop.FindPropertyRelative("Left"),
                prop.FindPropertyRelative("Right")
            };

            m_names = Enum.GetNames(keyFuncsEnum);
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

            for (int i = 0; i < m_directs.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(m_directs[i].displayName, GUILayout.MaxWidth(100f));
                m_directs[i].intValue = EditorGUILayout.Popup(m_directs[i].intValue, m_names);
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10f);

            if (GUI.changed)
                m_serializedObject.ApplyModifiedProperties();
        }
    }
}
