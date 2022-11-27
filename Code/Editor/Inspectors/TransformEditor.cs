using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Mathematics;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Inspectors
{
    [CustomEditor(typeof(Transform))]
    internal class TransformEditor : Editor<Transform>
    {
        private const string EDITOR_TYPE_NAME = "UnityEditor.TransformInspector";
        private const string UNDO_NAME = "Transform";
        private const string BUTTON_NAME = "X";
        private const float VERTICAL_OFFSET = 2f;

        private const string POSITION_LABEL = "Position";
        private const string ROTATION_LABEL = "Rotation";
        private const string SCALE_LABEL = "Scale";

        private Editor _builtInEditor;
        private SerializedProperty _posProp;
        private SerializedProperty _rotProp;
        private SerializedProperty _sclProp;

        private readonly string[] _toolbarNames = new string[] { "Local", "World" };
        private static bool _world;

        private readonly Rect _sceneGuiArea = new Rect(5f, 5f, 65f, 100f);
        private readonly string _pivotModeWarning = $"→ {PivotMode.Center}";
        private readonly string _pivotRotationWarning = $"→ {PivotRotation.Global}";

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
            Type type = Assembly.GetAssembly(typeof(Editor))
                                .GetType(EDITOR_TYPE_NAME);
            _builtInEditor = CreateEditor(target, type);

            _posProp = serializedObject.FindProperty("m_LocalPosition");
            _rotProp = serializedObject.FindProperty("m_LocalRotation");
            _sclProp = serializedObject.FindProperty("m_LocalScale");

#if UNITY_2021_1_OR_NEWER
            Tools.pivotModeChanged += SceneView.RepaintAll;
            Tools.pivotRotationChanged += SceneView.RepaintAll;
#endif
        }

        private void OnDisable()
        {
#if UNITY_2021_1_OR_NEWER
            Tools.pivotModeChanged -= SceneView.RepaintAll;
            Tools.pivotRotationChanged -= SceneView.RepaintAll;
#endif
            DestroyImmediate(_builtInEditor);
        }

        public override void OnInspectorGUI()
        {
            DrawPanel();

            if (_world)
            {
                DrawGlobal();
                return;
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(_areaOptions);
            DrawButtons();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            _builtInEditor.OnInspectorGUI();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void OnSceneGUI()
        {
            Handles.BeginGUI();
            GUILayout.BeginArea(_sceneGuiArea);

            if (Tools.pivotMode != PivotMode.Pivot)
            {
                GUI.color = Colours.Cyan;
                GUILayout.Label(_pivotModeWarning, EditorStylesExt.Rect, GUILayout.Height(25f));
            }

            if (Tools.pivotRotation != PivotRotation.Local)
            {
                GUI.color = Colours.Yellow;
                GUILayout.Label(_pivotRotationWarning, EditorStylesExt.Rect, GUILayout.Height(25f));
            }

            GUI.color = Colours.White;
            GUILayout.EndArea();
            Handles.EndGUI();
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

            GUILayout.Space(VERTICAL_OFFSET);

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
            GUILayout.Space(VERTICAL_OFFSET);

            if (drawLine(POSITION_LABEL, target.position, out Vector3 pos))
                target.position = pos;

            if (drawLine(ROTATION_LABEL, target.eulerAngles, out Vector3 rot))
                target.eulerAngles = rot;

            drawLine(SCALE_LABEL, target.lossyScale, out _, true);

            bool drawLine(string label, in Vector3 curValue, out Vector3 newValue, bool locked = false)
            {
                bool changed = false;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label, _labelOptions);
                if (locked)
                {
                    GUI.enabled = false;
                    EditorGUILayout.Vector3Field(GUIContent.none, curValue);
                    GUI.enabled = true;
                    newValue = default;
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    newValue = EditorGUILayout.Vector3Field(GUIContent.none, curValue);
                    if (changed = EditorGUI.EndChangeCheck())
                        Undo.RecordObject(target, UNDO_NAME);
                }
                EditorGUILayout.EndHorizontal();
                return changed;
            }
        }
    }
}
