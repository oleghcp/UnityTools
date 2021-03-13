﻿using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtility.Controls;
using UnityUtility.Controls.ControlStuff;
using UnityUtilityEditor.Window;

namespace UnityUtilityEditor.CustomEditors.InputLayouts
{
    [CustomEditor(typeof(LayoutConfig))]
    internal class LayoutConfigEditor : Editor<LayoutConfig>
    {
        private const float MAX_COLUMN_WIDTH = 200f;
        private const float MIN_COLUMN_WIDTH = 10f;

        private TypeSelector _keyEnumTypeSel;
        private TypeSelector _axisEnumTypeSel;

        private TypeValue _keyEnumTypeVal;
        private TypeValue _axisEnumTypeVal;

        private SerializedProperty _keyIndices;
        private SerializedProperty _axisIndices;

        private SerializedProperty _inputType;
        private bool _pretty;

        private KeyAxesWindow _keyAxesPopup;

        private void Awake()
        {
            _keyEnumTypeVal = new TypeValue(LayoutConfig.KeyEnumTypeFieldName);
            _axisEnumTypeVal = new TypeValue(LayoutConfig.AxisEnumTypeFieldName);

            InitTypeValue(_keyEnumTypeVal);
            InitTypeValue(_axisEnumTypeVal);

            _inputType = serializedObject.FindProperty(LayoutConfig.InputTypeFieldName);

            _keyIndices = serializedObject.FindProperty(LayoutConfig.KeyIndicesFieldName);
            _axisIndices = serializedObject.FindProperty(LayoutConfig.AxisIndicesFieldName);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (_inputType.enumValueIndex == (int)InputType.None)
            {
                DrawTypeChoice();
            }
            else
            {
                if (_keyEnumTypeVal.IsEmpty)
                {
                    DrawEnumChoice(ref _keyEnumTypeSel, _keyEnumTypeVal, "Choose your button action enum:");
                }
                else
                {
                    if (ChekKeys())
                        serializedObject.ApplyModifiedProperties();

                    int length = _keyEnumTypeVal.EnumNames.Length;
                    DrawKeys(length);
                    if (_keyEnumTypeVal.Toggles.Any())
                        DrawKeysButtons(length);
                }

                if (_axisEnumTypeVal.IsEmpty)
                {
                    DrawEnumChoice(ref _axisEnumTypeSel, _axisEnumTypeVal, "Choose your axis action enum:");
                }
                else
                {
                    if (ChekAxes())
                        serializedObject.ApplyModifiedProperties();

                    int length = _axisEnumTypeVal.EnumNames.Length;
                    DrawAxes(length);
                    if (_axisEnumTypeVal.Toggles.Any())
                        DrawAxesButtons(length);
                }

                if (_keyEnumTypeVal.IsEmpty || _axisEnumTypeVal.IsEmpty)
                {
                    GUILayout.Space(10f);
                    EditorGUILayout.HelpBox("Enumerations which represent game actions (jump, shoot, move etc.). These enums must have consecutive int values: 0, 1, 2 etc.", MessageType.Info);
                }
                else
                {
                    GUILayout.Space(20f);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10f);
                    if (GUILayout.Button("Print Json", GUILayout.MaxWidth(300f)))
                    {
                        object layout = target.ToBindLayout();
                        string json = JsonUtility.ToJson(layout, _pretty);
                        Debug.Log(json);
                    }
                    GUILayout.Space(10f);
                    _pretty = GUILayout.Toggle(_pretty, "Pretty", GUILayout.MaxWidth(60f));
                    EditorGUILayout.EndHorizontal();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        // -- //

        private bool IsKeyMouse()
        {
            return _inputType.enumValueIndex == (int)InputType.KeyMouse;
        }

        private void InitTypeValue(TypeValue typeValue)
        {
            string enumName = serializedObject.FindProperty(typeValue.PropName).stringValue ?? string.Empty;
            typeValue.SetValue(Type.GetType(enumName));
        }

        private void DrawTypeChoice()
        {
            GUILayout.Space(10f);
            EditorGuiLayout.CenterLabel("Choose control type:", EditorStyles.boldLabel);
            GUILayout.Space(5f);

            if (EditorGuiLayout.CenterButton("Keyboard and Mouse", GUILayout.Height(30f), GUILayout.MinWidth(150f)))
                _inputType.enumValueIndex = (int)InputType.KeyMouse;

            GUILayout.Space(5f);

            if (EditorGuiLayout.CenterButton("Gamepad", GUILayout.Height(30f), GUILayout.MinWidth(150f)))
                _inputType.enumValueIndex = (int)InputType.Gamepad;
        }

        private void DrawEnumChoice(ref TypeSelector selector, TypeValue typeValue, string label)
        {
            GUILayout.Space(10f);

            if (selector == null)
            {
                selector = new TypeSelector();
                Assembly[] assemblies = EditorUtilityExt.GetAssemblies();
                selector.Types = EditorUtilityExt.GetTypes(assemblies, type => type.IsSubclassOf(typeof(Enum)));
                if (selector.Types.Length > 0)
                {
                    selector.Types = selector.Types.OrderBy(itm => itm.FullName).ToArray();
                    selector.TypeNames = selector.Types.Select(EditorGuiUtility.GetTypeDisplayName).ToArray();
                }
            }

            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            if (selector.Types.Length > 0)
            {
                selector.Selected = EditorGuiLayout.DropDown(selector.Selected, selector.TypeNames);
                GUILayout.Space(5f);
                if (EditorGuiLayout.CenterButton("Apply", GUILayout.Height(30f), GUILayout.MinWidth(100f)))
                {
                    typeValue.SetValue(selector.Types[selector.Selected]);
                    string typeName = typeValue.EnumType.GetTypeName();
                    serializedObject.FindProperty(typeValue.PropName).stringValue = typeName;
                    selector = null;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Enum not found.", MessageType.Warning);
            }
        }

        private void DrawKeys(int length)
        {
            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Actions", EditorStyles.boldLabel, GUILayout.MaxWidth(MAX_COLUMN_WIDTH), GUILayout.MinWidth(MIN_COLUMN_WIDTH));
            GUILayout.Label("KeyCodes", EditorStyles.boldLabel, GUILayout.MaxWidth(MAX_COLUMN_WIDTH), GUILayout.MinWidth(MIN_COLUMN_WIDTH));

            bool all = _keyEnumTypeVal.Toggles.All();
            _keyEnumTypeVal.AllToggles = EditorGUILayout.Toggle(all, GUILayout.MaxWidth(EditorGuiUtility.SmallButtonWidth));
            if (_keyEnumTypeVal.AllToggles != all)
                _keyEnumTypeVal.Toggles.SetAll(_keyEnumTypeVal.AllToggles);

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5f);

            for (int i = 0; i < length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(_keyEnumTypeVal.EnumNames[i], GUILayout.MaxWidth(MAX_COLUMN_WIDTH), GUILayout.MinWidth(MIN_COLUMN_WIDTH));
                SerializedProperty keyIndexItem = _keyIndices.GetArrayElementAtIndex(i);

                if (IsKeyMouse())
                    keyIndexItem.intValue = (int)(KMKeyCode)EditorGUILayout.EnumPopup((KMKeyCode)keyIndexItem.intValue, GUILayout.MaxWidth(MAX_COLUMN_WIDTH));
                else
                    keyIndexItem.intValue = (int)(GPKeyCode)EditorGUILayout.EnumPopup((GPKeyCode)keyIndexItem.intValue, GUILayout.MaxWidth(MAX_COLUMN_WIDTH));

                _keyEnumTypeVal.Toggles[i] = EditorGUILayout.Toggle(_keyEnumTypeVal.Toggles[i], GUILayout.MaxWidth(EditorGuiUtility.SmallButtonWidth));

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5f);

            if (IsKeyMouse())
            {
                if (GUILayout.Button("Set Key Axes", GUILayout.MaxWidth(150f)))
                {
                    if (_keyAxesPopup == null)
                        (_keyAxesPopup = KeyAxesWindow.Create()).SetUp(serializedObject, _keyEnumTypeVal.EnumType);
                    else
                        _keyAxesPopup.Focus();
                }

                GUILayout.Space(5f);
            }
        }

        private void DrawAxes(int length)
        {
            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Actions", EditorStyles.boldLabel, GUILayout.MaxWidth(MAX_COLUMN_WIDTH), GUILayout.MinWidth(MIN_COLUMN_WIDTH));
            GUILayout.Label("AxisCodes", EditorStyles.boldLabel, GUILayout.MaxWidth(MAX_COLUMN_WIDTH), GUILayout.MinWidth(MIN_COLUMN_WIDTH));

            bool all = _axisEnumTypeVal.Toggles.All();
            _axisEnumTypeVal.AllToggles = EditorGUILayout.Toggle(all, GUILayout.MaxWidth(EditorGuiUtility.SmallButtonWidth));
            if (_axisEnumTypeVal.AllToggles != all)
                _axisEnumTypeVal.Toggles.SetAll(_axisEnumTypeVal.AllToggles);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);

            for (int i = 0; i < length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(_axisEnumTypeVal.EnumNames[i], GUILayout.MaxWidth(MAX_COLUMN_WIDTH), GUILayout.MinWidth(MIN_COLUMN_WIDTH));
                SerializedProperty axisIndexItem = _axisIndices.GetArrayElementAtIndex(i);

                if (IsKeyMouse())
                    axisIndexItem.intValue = (int)(KMAxisCode)EditorGUILayout.EnumPopup((KMAxisCode)axisIndexItem.intValue, GUILayout.MaxWidth(MAX_COLUMN_WIDTH));
                else
                    axisIndexItem.intValue = (int)(GPAxisCode)EditorGUILayout.EnumPopup((GPAxisCode)axisIndexItem.intValue, GUILayout.MaxWidth(MAX_COLUMN_WIDTH));

                _axisEnumTypeVal.Toggles[i] = EditorGUILayout.Toggle(_axisEnumTypeVal.Toggles[i], GUILayout.MaxWidth(EditorGuiUtility.SmallButtonWidth));

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5f);
        }

        private void DrawKeysButtons(int length)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Move Up", GUILayout.MaxWidth(150f)))
            {
                int defVal = InputEnum.GetKeyDefVal(_inputType.enumValueIndex);
                LayoutEditorUtility.MoveElements(_keyIndices, _keyEnumTypeVal.Toggles, true, defVal);
                LayoutEditorUtility.MoveToggles(_keyEnumTypeVal.Toggles, _keyIndices.arraySize, true);
            }

            if (GUILayout.Button("Move Down", GUILayout.MaxWidth(150f)))
            {
                int defVal = InputEnum.GetKeyDefVal(_inputType.enumValueIndex);
                LayoutEditorUtility.MoveElements(_keyIndices, _keyEnumTypeVal.Toggles, false, defVal);
                LayoutEditorUtility.MoveToggles(_keyEnumTypeVal.Toggles, _keyIndices.arraySize, false);
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("Clear", GUILayout.MaxWidth(80f)))
            {
                int defVal = InputEnum.GetKeyDefVal(_inputType.enumValueIndex);

                for (int i = 0; i < length; i++)
                {
                    if (_keyEnumTypeVal.Toggles[i])
                        _keyIndices.GetArrayElementAtIndex(i).intValue = defVal;
                }

                _keyEnumTypeVal.Toggles.SetAll(false);
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);
        }

        private void DrawAxesButtons(int length)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Move Up", GUILayout.MaxWidth(150f)))
            {
                int defVal = InputEnum.GetAxisDefVal(_inputType.enumValueIndex);
                LayoutEditorUtility.MoveElements(_axisIndices, _axisEnumTypeVal.Toggles, true, defVal);
                LayoutEditorUtility.MoveToggles(_axisEnumTypeVal.Toggles, _axisIndices.arraySize, true);
            }

            if (GUILayout.Button("Move Down", GUILayout.MaxWidth(150f)))
            {
                int defVal = InputEnum.GetAxisDefVal(_inputType.enumValueIndex);
                LayoutEditorUtility.MoveElements(_axisIndices, _axisEnumTypeVal.Toggles, false, defVal);
                LayoutEditorUtility.MoveToggles(_axisEnumTypeVal.Toggles, _axisIndices.arraySize, false);
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("Clear", GUILayout.MaxWidth(80f)))
            {
                int defVal = InputEnum.GetAxisDefVal(_inputType.enumValueIndex);

                for (int i = 0; i < length; i++)
                {
                    if (_axisEnumTypeVal.Toggles[i])
                        _axisIndices.GetArrayElementAtIndex(i).intValue = defVal;
                }

                _axisEnumTypeVal.Toggles.SetAll(false);
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);
        }

        private bool ChekKeys()
        {
            int targetSize = _keyEnumTypeVal.EnumNames.Length;

            int defVal = InputEnum.GetKeyDefVal(_inputType.enumValueIndex);
            return LayoutEditorUtility.EqualizeSize(_keyIndices, targetSize, defVal);
        }

        private bool ChekAxes()
        {
            int targetSize = _axisEnumTypeVal.EnumNames.Length;
            int defVal = InputEnum.GetAxisDefVal(_inputType.enumValueIndex);
            return LayoutEditorUtility.EqualizeSize(_axisIndices, targetSize, defVal);
        }

        private class TypeSelector
        {
            public Type[] Types;
            public string[] TypeNames;
            public int Selected;
        }

        private class TypeValue
        {
            public Type EnumType;
            public string PropName;
            public string[] EnumNames;
            public BitList Toggles;
            public bool AllToggles;

            public bool IsEmpty
            {
                get { return EnumType == null; }
            }

            public TypeValue(string propName)
            {
                PropName = propName;
            }

            public void SetValue(Type type)
            {
                EnumType = type;
                if (EnumType != null)
                {
                    EnumNames = Enum.GetNames(EnumType);
                    Toggles = new BitList(EnumNames.Length);
                }
            }
        }
    }
}
