using System;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityUtility.Controls;
using UnityUtility.Controls.ControlStuff;
using UnityUtility.Collections;
using UnityUtilityEditor.Window;
using Tools;

namespace UnityUtilityEditor.Input
{
    [CustomEditor(typeof(LayoutConfig))]
    internal class LayoutConfigEditor : Editor
    {
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
            public BitArrayMask Toggles;
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
                    Toggles = new BitArrayMask(EnumNames.Length);
                }
            }
        }

        private const float ACTION_WIDTH = 100f;
        private const float CODE_WIDTH = 120f;

        private TypeSelector m_keyEnumTypeSel;
        private TypeSelector m_axisEnumTypeSel;

        private TypeValue m_keyEnumTypeVal;
        private TypeValue m_axisEnumTypeVal;

        private SerializedProperty m_keyIndices;
        private SerializedProperty m_axisIndices;

        private SerializedProperty m_inputType;
        private bool m_pretty;

        private KeyAxesWindow m_keyAxesPopup;

        private void Awake()
        {
            m_keyEnumTypeVal = new TypeValue("_keyEnumType");
            m_axisEnumTypeVal = new TypeValue("_axisEnumType");

            f_initTypeValue(m_keyEnumTypeVal);
            f_initTypeValue(m_axisEnumTypeVal);

            m_inputType = serializedObject.FindProperty("InputType");

            m_keyIndices = serializedObject.FindProperty("KeyIndices");
            m_axisIndices = serializedObject.FindProperty("AxisIndices");
        }

        public override void OnInspectorGUI()
        {
            if (m_inputType.enumValueIndex == (int)InputType.None)
            {
                f_drawTypeChoice();
            }
            else
            {
                if (m_keyEnumTypeVal.IsEmpty)
                {
                    f_drawEnumChoice(ref m_keyEnumTypeSel, m_keyEnumTypeVal, "Choose your button action enum:");
                }
                else
                {
                    if (f_chekKeys())
                        serializedObject.ApplyModifiedProperties();

                    int length = m_keyEnumTypeVal.EnumNames.Length;
                    f_drawKeys(length);
                    if (m_keyEnumTypeVal.Toggles.Any())
                        f_drawKeysButtons(length);
                }

                if (m_axisEnumTypeVal.IsEmpty)
                {
                    f_drawEnumChoice(ref m_axisEnumTypeSel, m_axisEnumTypeVal, "Choose your axis action enum:");
                }
                else
                {
                    if (f_chekAxes())
                        serializedObject.ApplyModifiedProperties();

                    int length = m_axisEnumTypeVal.EnumNames.Length;
                    f_drawAxes(length);
                    if (m_axisEnumTypeVal.Toggles.Any())
                        f_drawAxesButtons(length);
                }

                if (m_keyEnumTypeVal.IsEmpty || m_axisEnumTypeVal.IsEmpty)
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
                        object layout = (target as LayoutConfig).ToBindLayout();
                        string json = JsonUtility.ToJson(layout, m_pretty);
                        Debug.Log(json);
                    }
                    GUILayout.Space(10f);
                    m_pretty = GUILayout.Toggle(m_pretty, "Pretty", GUILayout.MaxWidth(60f));
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        // -- //

        private bool f_isKeyMouse()
        {
            return m_inputType.enumValueIndex == (int)InputType.KeyMouse;
        }

        private void f_initTypeValue(TypeValue typeValue)
        {
            string enumName = serializedObject.FindProperty(typeValue.PropName).stringValue ?? string.Empty;
            typeValue.SetValue(Type.GetType(enumName));
        }

        private void f_drawTypeChoice()
        {
            GUILayout.Space(10f);
            EditorScriptUtility.DrawCenterLabel("Choose control type:", 150f);
            GUILayout.Space(5f);

            if (EditorScriptUtility.DrawCenterButton("Keyboard and Mouse", 150f, 30f))
                m_inputType.enumValueIndex = (int)InputType.KeyMouse;

            GUILayout.Space(5f);

            if (EditorScriptUtility.DrawCenterButton("Gamepad", 150f, 30f))
                m_inputType.enumValueIndex = (int)InputType.Gamepad;
        }

        private void f_drawEnumChoice(ref TypeSelector selector, TypeValue typeValue, string label)
        {
            GUILayout.Space(10f);

            if (selector == null)
            {
                selector = new TypeSelector();
                var ass = EditorUtilityExt.GetAssemblies();
                selector.Types = EditorUtilityExt.GetTypes(ass, type => type.IsSubclassOf(typeof(Enum)));
                if (selector.Types.Length > 0)
                {
                    selector.Types = selector.Types.OrderBy(itm => itm.FullName).ToArray();
                    selector.TypeNames = selector.Types.Select(itm => itm.FullName).ToArray();
                }
            }

            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            if (selector.Types.Length > 0)
            {
                selector.Selected = EditorGUILayout.Popup(selector.Selected, selector.TypeNames);
                GUILayout.Space(5f);
                if (EditorScriptUtility.DrawCenterButton("Apply", 100f, 30f))
                {
                    typeValue.SetValue(selector.Types[selector.Selected]);
                    string typeName = Helper.CutAssemblyQualifiedName(typeValue.EnumType.AssemblyQualifiedName);
                    serializedObject.FindProperty(typeValue.PropName).stringValue = typeName;
                    selector = null;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Enum not found.", MessageType.Warning);
            }
        }

        private void f_drawKeys(int length)
        {
            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel, GUILayout.MaxWidth(ACTION_WIDTH));
            EditorGUILayout.LabelField("KeyCodes", EditorStyles.boldLabel, GUILayout.MaxWidth(CODE_WIDTH), GUILayout.MinWidth(10f));

            bool all = m_keyEnumTypeVal.Toggles.All();
            m_keyEnumTypeVal.AllToggles = EditorGUILayout.Toggle(all, GUILayout.MaxWidth(20f));
            if (m_keyEnumTypeVal.AllToggles != all)
                m_keyEnumTypeVal.Toggles.SetAll(m_keyEnumTypeVal.AllToggles);

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5f);

            for (int i = 0; i < length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(m_keyEnumTypeVal.EnumNames[i], GUILayout.MaxWidth(100f));
                SerializedProperty keyIndexItem = m_keyIndices.GetArrayElementAtIndex(i);

                if (f_isKeyMouse())
                    keyIndexItem.intValue = (int)(KMKeyCode)EditorGUILayout.EnumPopup((KMKeyCode)keyIndexItem.intValue, GUILayout.MaxWidth(CODE_WIDTH));
                else
                    keyIndexItem.intValue = (int)(GPKeyCode)EditorGUILayout.EnumPopup((GPKeyCode)keyIndexItem.intValue, GUILayout.MaxWidth(CODE_WIDTH));

                m_keyEnumTypeVal.Toggles[i] = EditorGUILayout.Toggle(m_keyEnumTypeVal.Toggles[i], GUILayout.MaxWidth(20f));

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5f);

            if (f_isKeyMouse())
            {
                if (GUILayout.Button("Set Key Axes", GUILayout.MaxWidth(150f)))
                {
                    if (m_keyAxesPopup == null)
                        (m_keyAxesPopup = KeyAxesWindow.Create()).SetUp(serializedObject, m_keyEnumTypeVal.EnumType);
                    else
                        m_keyAxesPopup.Focus();
                }

                GUILayout.Space(5f);
            }
        }

        private void f_drawAxes(int length)
        {
            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel, GUILayout.MaxWidth(ACTION_WIDTH));
            EditorGUILayout.LabelField("AxisCodes", EditorStyles.boldLabel, GUILayout.MaxWidth(CODE_WIDTH), GUILayout.MinWidth(10f));

            bool all = m_axisEnumTypeVal.Toggles.All();
            m_axisEnumTypeVal.AllToggles = EditorGUILayout.Toggle(all, GUILayout.MaxWidth(20f));
            if (m_axisEnumTypeVal.AllToggles != all)
                m_axisEnumTypeVal.Toggles.SetAll(m_axisEnumTypeVal.AllToggles);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);

            for (int i = 0; i < length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(m_axisEnumTypeVal.EnumNames[i], GUILayout.MaxWidth(100f));
                SerializedProperty axisIndexItem = m_axisIndices.GetArrayElementAtIndex(i);

                if (f_isKeyMouse())
                    axisIndexItem.intValue = (int)(KMAxisCode)EditorGUILayout.EnumPopup((KMAxisCode)axisIndexItem.intValue, GUILayout.MaxWidth(CODE_WIDTH));
                else
                    axisIndexItem.intValue = (int)(GPAxisCode)EditorGUILayout.EnumPopup((GPAxisCode)axisIndexItem.intValue, GUILayout.MaxWidth(CODE_WIDTH));

                m_axisEnumTypeVal.Toggles[i] = EditorGUILayout.Toggle(m_axisEnumTypeVal.Toggles[i], GUILayout.MaxWidth(20f));

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5f);
        }

        private void f_drawKeysButtons(int length)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Move Up", GUILayout.MaxWidth(150f)))
            {
                int defVal = InputEnum.GetKeyDefVal(m_inputType.enumValueIndex);
                EditorScriptUtility.MoveElements(m_keyIndices, m_keyEnumTypeVal.Toggles, true, defVal);
                EditorScriptUtility.MoveToggles(m_keyEnumTypeVal.Toggles, m_keyIndices.arraySize, true);
            }

            if (GUILayout.Button("Move Down", GUILayout.MaxWidth(150f)))
            {
                int defVal = InputEnum.GetKeyDefVal(m_inputType.enumValueIndex);
                EditorScriptUtility.MoveElements(m_keyIndices, m_keyEnumTypeVal.Toggles, false, defVal);
                EditorScriptUtility.MoveToggles(m_keyEnumTypeVal.Toggles, m_keyIndices.arraySize, false);
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("Clear", GUILayout.MaxWidth(80f)))
            {
                int defVal = InputEnum.GetKeyDefVal(m_inputType.enumValueIndex);

                for (int i = 0; i < length; i++)
                {
                    if (m_keyEnumTypeVal.Toggles[i])
                        m_keyIndices.GetArrayElementAtIndex(i).intValue = defVal;
                }

                m_keyEnumTypeVal.Toggles.SetAll(false);
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);
        }

        private void f_drawAxesButtons(int length)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Move Up", GUILayout.MaxWidth(150f)))
            {
                int defVal = InputEnum.GetAxisDefVal(m_inputType.enumValueIndex);
                EditorScriptUtility.MoveElements(m_axisIndices, m_axisEnumTypeVal.Toggles, true, defVal);
                EditorScriptUtility.MoveToggles(m_axisEnumTypeVal.Toggles, m_axisIndices.arraySize, true);
            }

            if (GUILayout.Button("Move Down", GUILayout.MaxWidth(150f)))
            {
                int defVal = InputEnum.GetAxisDefVal(m_inputType.enumValueIndex);
                EditorScriptUtility.MoveElements(m_axisIndices, m_axisEnumTypeVal.Toggles, false, defVal);
                EditorScriptUtility.MoveToggles(m_axisEnumTypeVal.Toggles, m_axisIndices.arraySize, false);
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("Clear", GUILayout.MaxWidth(80f)))
            {
                int defVal = InputEnum.GetAxisDefVal(m_inputType.enumValueIndex);

                for (int i = 0; i < length; i++)
                {
                    if (m_axisEnumTypeVal.Toggles[i])
                        m_axisIndices.GetArrayElementAtIndex(i).intValue = defVal;
                }

                m_axisEnumTypeVal.Toggles.SetAll(false);
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);
        }

        private bool f_chekKeys()
        {
            int targetSize = m_keyEnumTypeVal.EnumNames.Length;

            int defVal = InputEnum.GetKeyDefVal(m_inputType.enumValueIndex);
            return EditorScriptUtility.EqualizeSize(m_keyIndices, targetSize, defVal);
        }

        private bool f_chekAxes()
        {
            int targetSize = m_axisEnumTypeVal.EnumNames.Length;
            int defVal = InputEnum.GetAxisDefVal(m_inputType.enumValueIndex);
            return EditorScriptUtility.EqualizeSize(m_axisIndices, targetSize, defVal);
        }
    }
}
