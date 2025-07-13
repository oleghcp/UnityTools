using System;
using System.Reflection;
using OlegHcp;
using OlegHcp.Engine;
using OlegHcp.Mathematics;
using OlegHcpEditor.Engine;
using OlegHcpEditor.MenuItems;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Inspectors
{
    [CustomEditor(typeof(Transform))]
    internal class TransformEditor : Editor<Transform>
    {
        private static readonly string UNDO_NAME = "Transform";
        private static readonly string BUTTON_NAME = "X";
        private readonly float VERTICAL_OFFSET = 2f;

        private readonly string POSITION_LABEL = "Position";
        private readonly string ROTATION_LABEL = "Rotation";
        private readonly string SCALE_LABEL = "Scale";

        private static readonly string _pivotModeWarning = $"→ {PivotMode.Center}";
        private static readonly string _pivotRotationWarning = $"→ {PivotRotation.Global}";

        private static readonly string[] _toolbarNames = new[]
        {
            "Local",
            "World",
        };

        private static readonly GUILayoutOption[] _buttonOptions = new[]
        {
            GUILayout.Height(EditorGUIUtility.singleLineHeight),
            GUILayout.Width(EditorGUIUtility.singleLineHeight),
        };

        private static readonly GUILayoutOption[] _areaOptions = new[]
        {
            GUILayout.Width(EditorGUIUtility.singleLineHeight),
        };

        private static readonly GUILayoutOption[] _labelOptions = new[]
        {
            GUILayout.Width(60f),
        };

        private Editor _builtInEditor;
        private static bool _world;

        private void OnEnable()
        {
            Type type = Assembly.GetAssembly(typeof(Editor))
                                .GetType("UnityEditor.TransformInspector");
            _builtInEditor = CreateEditor(target, type);
        }

        private void OnDisable()
        {
            _builtInEditor.DestroyImmediate();
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            SceneView.duringSceneGui += OnDuringSceneGui;
        }

        public override void OnInspectorGUI()
        {
            DrawToolbar();

            bool editable = !target.HasHideFlag(HideFlags.NotEditable);

            if (_world)
            {
                DrawGlobal(editable);
                return;
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(_areaOptions);
            DrawResetButtons(editable);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            _builtInEditor.OnInspectorGUI();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawToolbar()
        {
            if (targets.Count <= 1 && target.parent)
            {
                _world = GUILayout.Toolbar(_world.ToInt(), _toolbarNames) > 0;
                return;
            }

            GUI.enabled = false;
            _world = false;
            GUILayout.Toolbar(0, _toolbarNames);
            GUI.enabled = true;
        }

        private void DrawResetButtons(bool enabled)
        {
            serializedObject.Update();

            GUI.enabled = enabled;

            GUILayout.Space(VERTICAL_OFFSET);

            if (GUILayout.Button(BUTTON_NAME, _buttonOptions))
                getProp("m_LocalPosition").vector3Value = Vector3.zero;

            if (GUILayout.Button(BUTTON_NAME, _buttonOptions))
            {
                getProp("m_LocalRotation").quaternionValue = Quaternion.identity;
                getProp("m_LocalEulerAnglesHint").vector3Value = Vector3.zero;
            }

            if (GUILayout.Button(BUTTON_NAME, _buttonOptions))
                getProp("m_LocalScale").vector3Value = Vector3.one;

            GUI.enabled = true;

            serializedObject.ApplyModifiedProperties();

            SerializedProperty getProp(string name)
            {
                return serializedObject.FindProperty(name);
            }
        }

        private void DrawGlobal(bool enabled)
        {
            GUI.enabled = enabled;

            GUILayout.Space(VERTICAL_OFFSET);

            if (drawLine(POSITION_LABEL, target.position, out Vector3 pos))
                target.position = pos;

            if (drawLine(ROTATION_LABEL, target.eulerAngles, out Vector3 rot))
                target.eulerAngles = rot;

            drawLine(SCALE_LABEL, target.lossyScale, out _, true);

            GUI.enabled = true;

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

        private static void OnDuringSceneGui(SceneView sceneView)
        {
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(5f, 5f, 60f, 35f));

            if (Tools.pivotMode != PivotMode.Pivot)
            {
                GUI.color = Colours.Cyan;
                GUILayout.Label(_pivotModeWarning);
            }

            if (Tools.pivotRotation != PivotRotation.Local)
            {
                GUI.color = Colours.Yellow;
                GUILayout.Label(_pivotRotationWarning);
            }

            GUI.color = Colours.White;
            GUILayout.EndArea();
            Handles.EndGUI();
        }

        [MenuItem(MenuItemsUtility.CONTEXT_MENU_NAME + nameof(Transform) + "/Copy/To Clipboard (ext.)")]
        private static void CopyToClipboard(MenuCommand command)
        {
            GUIUtility.systemCopyBuffer = EditorJsonUtility.ToJson(command.context);
        }

        private const string pastFromClipboardMenuName = MenuItemsUtility.CONTEXT_MENU_NAME + nameof(Transform) + "/Paste/From Clipboard (ext.)";

        [MenuItem(pastFromClipboardMenuName)]
        private static void PastFromClipboard(MenuCommand command)
        {
            try
            {
                Undo.RecordObject(command.context, "Paste Values from Clipboard");
                EditorJsonUtility.FromJsonOverwrite(GUIUtility.systemCopyBuffer, command.context);
            }
            catch (ArgumentException e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        [MenuItem(pastFromClipboardMenuName, true)]
        private static bool PastFromClipboardValidate(MenuCommand command)
        {
            return !command.context.HasHideFlag(HideFlags.NotEditable);
        }
    }
}
