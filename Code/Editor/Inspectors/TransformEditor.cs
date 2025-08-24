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
        private static readonly TransformEditorOptions _options = new TransformEditorOptions();

        private const float VERTICAL_OFFSET = 2f;

        private readonly string PositionLabel = "Position";
        private readonly string RotationLabel = "Rotation";
        private readonly string ScaleLabel = "Scale";

        private Editor _builtInEditor;

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

            if (_options.World)
            {
                DrawGlobal(editable);
                return;
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(_options.AreaOptions);
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
                _options.World = GUILayout.Toolbar(_options.World.ToInt(), _options.ToolbarNames) > 0;
                return;
            }

            GUI.enabled = false;
            _options.World = false;
            GUILayout.Toolbar(0, _options.ToolbarNames);
            GUI.enabled = true;
        }

        private void DrawResetButtons(bool enabled)
        {
            serializedObject.Update();

            GUI.enabled = enabled;

            GUILayout.Space(VERTICAL_OFFSET);

            if (GUILayout.Button(_options.ButtonName, _options.ButtonOptions))
                getProp("m_LocalPosition").vector3Value = Vector3.zero;

            if (GUILayout.Button(_options.ButtonName, _options.ButtonOptions))
            {
                getProp("m_LocalRotation").quaternionValue = Quaternion.identity;
                getProp("m_LocalEulerAnglesHint").vector3Value = Vector3.zero;
            }

            if (GUILayout.Button(_options.ButtonName, _options.ButtonOptions))
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

            if (drawLine(PositionLabel, target.position, out Vector3 pos))
                target.position = pos;

            if (drawLine(RotationLabel, target.eulerAngles, out Vector3 rot))
                target.eulerAngles = rot;

            drawLine(ScaleLabel, target.lossyScale, out _, true);

            GUI.enabled = true;

            bool drawLine(string label, in Vector3 curValue, out Vector3 newValue, bool locked = false)
            {
                bool changed = false;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label, _options.LabelOptions);
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
                        Undo.RecordObject(target, _options.UndoName);
                }
                EditorGUILayout.EndHorizontal();
                return changed;
            }
        }

        private static void OnDuringSceneGui(SceneView sceneView)
        {
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(5f, 5f, 60f, 45f));

            if (Tools.pivotMode != PivotMode.Pivot)
            {
                GUI.color = Colours.Cyan;
                GUILayout.Label(_options.PivotModeWarning, EditorStylesExt.Rect, _options.ModeOptions);
            }

            if (Tools.pivotRotation != PivotRotation.Local)
            {
                GUI.color = Colours.Yellow;
                GUILayout.Label(_options.PivotRotationWarning, EditorStylesExt.Rect, _options.ModeOptions);
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
