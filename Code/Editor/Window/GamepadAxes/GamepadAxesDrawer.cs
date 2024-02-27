#if !UNITY_2019_3_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using OlegHcp.Controls.ControlStuff;
using OlegHcpEditor.Engine;

namespace OlegHcpEditor.Window.GamepadAxes
{
    internal class GamepadAxesDrawer
    {
        private SerializedObject _inputSettings;

        private SerializedProperty _axesArray;

        private List<string> _names = new List<string>();

        private Vector2 _scrollPos;

        private int _pads = 2;
        private int _axes = 15;

        public GamepadAxesDrawer()
        {
            _inputSettings = new SerializedObject(Managers.GetInputManager());
            _axesArray = _inputSettings.FindProperty("m_Axes");
            RefreshAxes();
        }

        public void OnGUI()
        {
            _inputSettings.Update();

            EditorGUILayout.Space();

            if (GUILayout.Button("Open Input Manager", GUILayout.MaxWidth(150f), GUILayout.Height(25f)))
                Selection.activeObject = _inputSettings.targetObject;

            GUILayout.Space(5f);

            _scrollPos.y = GUILayout.BeginScrollView(_scrollPos, EditorStyles.helpBox).y;
            for (int i = 0; i < _names.Count; i++)
            {
                EditorGUILayout.LabelField(_names[i], GUILayout.MaxWidth(150f), GUILayout.MinWidth(10f));
            }
            GUILayout.EndScrollView();

            GUILayout.Space(5f);

            EditorGUILayout.LabelField("Add gamepad axes:", EditorStyles.boldLabel);
            GUILayout.Space(5f);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Gamepads amount:", GUILayout.MaxWidth(150f), GUILayout.MinWidth(10f));
            _pads = EditorGUILayout.IntField(_pads, GUILayout.Width(30f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Axes amount:", GUILayout.MaxWidth(150f), GUILayout.MinWidth(10f));
            _axes = EditorGUILayout.IntField(_axes, GUILayout.Width(30f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            GUILayout.Space(5f);

            if (GUILayout.Button("Add axes", GUILayout.Width(190f), GUILayout.Height(25f)))
            {
                StringBuilder builder = new StringBuilder();

                for (int pad = 0; pad < _pads; pad++)
                {
                    for (int axis = 0; axis < _axes; axis++)
                    {
                        AddAxis(axis + 1, pad + 1, builder);
                    }
                }
                RefreshAxes();
            }

            GUILayout.Space(5f);

            if (GUILayout.Button("Remove axes", GUILayout.Height(25f), GUILayout.Width(190f)))
            {
                int i = 0;

                while (i < _axesArray.arraySize)
                {
                    SerializedProperty axis = _axesArray.GetArrayElementAtIndex(i);

                    if (axis.FindPropertyRelative("type").enumValueIndex != 2)
                    {
                        i++;
                        continue;
                    }

                    string name = axis.FindPropertyRelative("m_Name").stringValue;
                    int num = axis.FindPropertyRelative("joyNum").enumValueIndex;
                    bool propVal = name[0] - (char)num == '@' && name[1] == ':' && int.TryParse(name[2].ToString(), out _);

                    if (!propVal) { i++; }
                    else { _axesArray.DeleteArrayElementAtIndex(i); }
                }

                RefreshAxes();
            }

            GUILayout.Space(10f);

            _inputSettings.ApplyModifiedProperties();
        }

        private void RefreshAxes()
        {
            _names.Clear();

            foreach (var axis in _axesArray.EnumerateArrayElements())
            {
                _names.Add(axis.FindPropertyRelative("m_Name").stringValue);
            }
        }

        private void AddAxis(int axNum, int padNum, StringBuilder builder)
        {
            SerializedProperty axis = _axesArray.AddArrayElement();

            axis.FindPropertyRelative("m_Name").stringValue = InputUnility.AxisName(axNum, padNum, builder);
            axis.FindPropertyRelative("dead").floatValue = 0.1f;
            axis.FindPropertyRelative("sensitivity").floatValue = 1f;
            axis.FindPropertyRelative("type").enumValueIndex = 2;
            axis.FindPropertyRelative("axis").enumValueIndex = axNum - 1;
            axis.FindPropertyRelative("joyNum").enumValueIndex = padNum;
        }
    }
}
#endif
