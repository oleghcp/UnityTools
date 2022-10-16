using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.Inspectors
{
    [CustomEditor(typeof(Transform))]
    internal class TransformEditor2 : Editor<Transform>
    {
        private const string BUTTON_NAME = "X";
        private const float VERTICAL_BUTTON_OFFSET = 2f;

        private Editor _builtInEditor;
        private SerializedProperty _posProp;
        private SerializedProperty _rotProp;
        private SerializedProperty _sclProp;

        private GUILayoutOption[] _buttonOptions = new[]
        {
            GUILayout.Height(EditorGUIUtility.singleLineHeight),
            GUILayout.Width(EditorGUIUtility.singleLineHeight),
        };

        private GUILayoutOption[] _areaOptions = new[]
        {
            GUILayout.Width(EditorGUIUtility.singleLineHeight),
        };

        private void OnEnable()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Editor));
            Type type = assembly.GetType("UnityEditor.TransformInspector");
            _builtInEditor = CreateEditor(target, type);

            _posProp = serializedObject.FindProperty("m_LocalPosition");
            _rotProp = serializedObject.FindProperty("m_LocalRotation");
            _sclProp = serializedObject.FindProperty("m_LocalScale");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(_areaOptions);
            DrawButtons();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            _builtInEditor.OnInspectorGUI();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawButtons()
        {
            serializedObject.Update();

            GUILayout.Space(VERTICAL_BUTTON_OFFSET);

            if (GUILayout.Button(BUTTON_NAME, _buttonOptions))
                _posProp.vector3Value = Vector3.zero;

            if (GUILayout.Button(BUTTON_NAME, _buttonOptions))
                _rotProp.quaternionValue = Quaternion.identity;

            if (GUILayout.Button(BUTTON_NAME, _buttonOptions))
                _sclProp.vector3Value = Vector3.one;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
