using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityUtility.MathExt;

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

        private string[] _toolbarNames = new string[] { "Local", "World" };
        private static bool _world;

        private GUILayoutOption[] _buttonOptions = new[]
        {
            GUILayout.Height(EditorGUIUtility.singleLineHeight),
            GUILayout.Width(EditorGUIUtility.singleLineHeight),
        };

        private GUILayoutOption[] _areaOptions = new[]
        {
            GUILayout.Width(EditorGUIUtility.singleLineHeight),
        };

        private GUILayoutOption[] _labelOptions = new[]
        {
            GUILayout.Width(60f),
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
            DrawPanel();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(_areaOptions);
            DrawButtons();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            if (_world)
                DrawGlobal();
            else
                _builtInEditor.OnInspectorGUI();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawPanel()
        {
            bool hasParent = target.parent;

            GUI.enabled = hasParent;
            _world = GUILayout.Toolbar((hasParent && _world).ToInt(), _toolbarNames).ToBool();
            GUI.enabled = true;
        }

        private void DrawButtons()
        {
            serializedObject.Update();

            GUILayout.Space(VERTICAL_BUTTON_OFFSET);

            GUI.enabled = !_world;

            if (GUILayout.Button(BUTTON_NAME, _buttonOptions))
                _posProp.vector3Value = Vector3.zero;

            if (GUILayout.Button(BUTTON_NAME, _buttonOptions))
                _rotProp.quaternionValue = Quaternion.identity;

            if (GUILayout.Button(BUTTON_NAME, _buttonOptions))
                _sclProp.vector3Value = Vector3.one;

            GUI.enabled = true;

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGlobal()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Position", _labelOptions);
            GUI.enabled = false;
            EditorGUILayout.Vector3Field(GUIContent.none, target.position);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Rotation", _labelOptions);
            GUI.enabled = false;
            EditorGUILayout.Vector3Field(GUIContent.none, target.rotation.eulerAngles);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Scale", _labelOptions);
            GUI.enabled = false;
            EditorGUILayout.Vector3Field(GUIContent.none, target.lossyScale);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }
    }
}
