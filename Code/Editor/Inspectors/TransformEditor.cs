﻿using System;
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
        private const string UNDO_NAME = "Transform";
        private const string BUTTON_NAME = "X";
        private const float VERTICAL_OFFSET = 2f;

        private const string POSITION_LABEL = "Position";
        private const string ROTATION_LABEL = "Rotation";
        private const string SCALE_LABEL = "Scale";

        private Editor _builtInEditor;

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
                                .GetType("UnityEditor.TransformInspector");
            _builtInEditor = CreateEditor(target, type);

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
            _builtInEditor.DestroyImmediate();
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

        public override void OnInspectorGUI()
        {
            DrawToolbar();

            bool editable = !target.HasHideFlag(HideFlags.NotEditable);

            if (_world)
            {
                GUI.enabled = editable;
                DrawGlobal();
                GUI.enabled = true;
                return;
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(_areaOptions);
            GUI.enabled = editable;
            DrawResetButtons();
            GUI.enabled = true;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            _builtInEditor.OnInspectorGUI();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawToolbar()
        {
            bool hasParent = target.parent;
            GUI.enabled = hasParent;
            _world = GUILayout.Toolbar((hasParent && _world).ToInt(), _toolbarNames) > 0;
            GUI.enabled = true;
        }

        private void DrawResetButtons()
        {
            serializedObject.Update();

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

            serializedObject.ApplyModifiedProperties();

            SerializedProperty getProp(string name)
            {
                return serializedObject.FindProperty(name);
            }
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
