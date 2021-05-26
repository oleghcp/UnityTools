using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtility.Controls;
using UnityUtility.Controls.ControlStuff;
using UnityUtilityEditor.Window;

namespace UnityUtilityEditor.Inspectors.InputLayouts
{
    [CustomEditor(typeof(LayoutConfig))]
    internal class LayoutConfigEditor : Editor<LayoutConfig>
    {
        private TypeSelector _keyEnumTypeSel;
        private TypeSelector _axisEnumTypeSel;

        private TypeValue _keyEnumTypeVal;
        private TypeValue _axisEnumTypeVal;

        private SerializedProperty _keyIndices;
        private SerializedProperty _axisIndices;

        private SerializedProperty _inputType;

        private void Awake()
        {
            _inputType = serializedObject.FindProperty(LayoutConfig.InputTypeFieldName);

            _keyEnumTypeVal = new TypeValue(LayoutConfig.KeyEnumTypeFieldName);
            _axisEnumTypeVal = new TypeValue(LayoutConfig.AxisEnumTypeFieldName);

            InitTypeValue(_keyEnumTypeVal);
            InitTypeValue(_axisEnumTypeVal);

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
                    if (ChekValues(_keyEnumTypeVal, _keyIndices, InputEnumUtility.GetKeyDefVal))
                        serializedObject.ApplyModifiedProperties();

                    DrawLines<KMKeyCode, GPKeyCode>(_keyEnumTypeVal, _keyIndices);
                    DrawButtons(_keyEnumTypeVal, _keyIndices, InputEnumUtility.GetKeyDefVal);

                    EditorGUILayout.Space(5f);

                    if (IsKeyMouse())
                    {
                        if (EditorGUILayout.DropdownButton(EditorGuiUtility.TempContent("Set Key Axes"), FocusType.Keyboard, GUILayout.Width(100f)))
                            KeyAxesWindow.Create(serializedObject, _keyEnumTypeVal.EnumType);

                        GUILayout.Space(5f);
                    }
                }

                if (_axisEnumTypeVal.IsEmpty)
                {
                    DrawEnumChoice(ref _axisEnumTypeSel, _axisEnumTypeVal, "Choose your axis action enum:");
                }
                else
                {
                    if (ChekValues(_axisEnumTypeVal, _axisIndices, InputEnumUtility.GetAxisDefVal))
                        serializedObject.ApplyModifiedProperties();

                    DrawLines<KMAxisCode, GPAxisCode>(_axisEnumTypeVal, _axisIndices);
                    DrawButtons(_axisEnumTypeVal, _axisIndices, InputEnumUtility.GetAxisDefVal);
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
                    if (GUILayout.Button("Print Json"))
                        prinJson(false);
                    if (GUILayout.Button("Print Json Pretty"))
                        prinJson(true);
                    GUILayout.Space(10f);
                    EditorGUILayout.EndHorizontal();
                }
            }

            serializedObject.ApplyModifiedProperties();

            void prinJson(bool pretty)
            {
                object layout = target.ToBindLayout();
                string json = JsonUtility.ToJson(layout, pretty);
                Debug.Log(json);
            }
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
                selector.Types = EditorUtilityExt.GetTypes(assemblies, type => type.IsEnum);

                if (selector.Types.Length > 0)
                {
                    selector.Types.Sort(item => item.Name);
                    selector.TypeNames = selector.Types
                                                 .Select(EditorGuiUtility.GetTypeDisplayName)
                                                 .ToArray();
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

        private void DrawLines<TKMCode, TGPCode>(TypeValue typeValue, SerializedProperty indices) where TKMCode : Enum where TGPCode : Enum
        {
            GUILayout.Space(10f);

            using (new EditorGUILayout.HorizontalScope())
            {
                bool all = typeValue.Toggles.All();
                typeValue.AllToggles = EditorGUILayout.Toggle(all, GUILayout.MaxWidth(EditorGuiUtility.SmallButtonWidth));
                if (typeValue.AllToggles != all)
                    typeValue.Toggles.SetAll(typeValue.AllToggles);

                EditorGUILayout.LabelField("Actions", "Codes", EditorStyles.boldLabel);
            }

            GUILayout.Space(5f);

            Type enumType = IsKeyMouse() ? typeof(TKMCode) : typeof(TGPCode);
            int length = typeValue.EnumNames.Length;
            for (int i = 0; i < length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                typeValue.Toggles[i] = EditorGUILayout.Toggle(typeValue.Toggles[i], GUILayout.MaxWidth(EditorGuiUtility.SmallButtonWidth));

                EditorGUILayout.PrefixLabel(typeValue.EnumNames[i]);

                SerializedProperty indexProp = indices.GetArrayElementAtIndex(i);
                Enum enumValue = Enum.ToObject(enumType, indexProp.intValue) as Enum;
                enumValue = EditorGuiLayout.EnumDropDown(enumValue);
                indexProp.intValue = Convert.ToInt32(enumValue);

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5f);
        }

        private void DrawButtons(TypeValue typeValue, SerializedProperty indices, Func<int, int> defaultValue)
        {
            if (typeValue.Toggles.IsEmpty())
                return;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Move Up", GUILayout.MaxWidth(150f)))
            {
                int defVal = defaultValue(_inputType.enumValueIndex);
                LayoutEditorUtility.MoveElements(indices, typeValue.Toggles, true, defVal);
                LayoutEditorUtility.MoveToggles(typeValue.Toggles, indices.arraySize, true);
            }

            if (GUILayout.Button("Move Down", GUILayout.MaxWidth(150f)))
            {
                int defVal = defaultValue(_inputType.enumValueIndex);
                LayoutEditorUtility.MoveElements(indices, typeValue.Toggles, false, defVal);
                LayoutEditorUtility.MoveToggles(typeValue.Toggles, indices.arraySize, false);
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("Clear", GUILayout.MaxWidth(80f)))
            {
                int defVal = defaultValue(_inputType.enumValueIndex);
                int length = typeValue.EnumNames.Length;
                for (int i = 0; i < length; i++)
                {
                    if (typeValue.Toggles[i])
                        indices.GetArrayElementAtIndex(i).intValue = defVal;
                }

                typeValue.Toggles.SetAll(false);
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);
        }

        private bool ChekValues(TypeValue typeValue, SerializedProperty indices, Func<int, int> defaultValue)
        {
            int targetSize = typeValue.EnumNames.Length;
            int defVal = defaultValue(_inputType.enumValueIndex);
            return LayoutEditorUtility.EqualizeSize(indices, targetSize, defVal);
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

            public bool IsEmpty => EnumType == null;

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
