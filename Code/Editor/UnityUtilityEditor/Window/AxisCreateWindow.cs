using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityUtility.Controls.ControlStuff;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Window
{
    internal class AxisCreateWindow : EditorWindow
    {
        private SerializedObject _inputManager;
        private SerializedProperty _axesArray;
        private List<string> _names;

        private Vector2 _scrollPos;

        private int _pads = 2;
        private int _axes = 15;

        private void OnEnable()
        {
            minSize = new Vector2(300f, 300f);
            maxSize = new Vector2(300f, 1500f);

            UnityObject asset = AssetDatabase.LoadAssetAtPath<UnityObject>($"ProjectSettings/InputManager{EditorUtilityExt.ASSET_EXTENSION}");
            _inputManager = new SerializedObject(asset);
            _axesArray = GetAxes();
            _names = new List<string>();
            RefreshAxes();
        }

        private void OnGUI()
        {
            GUILayout.Space(5f);

            _scrollPos.y = GUILayout.BeginScrollView(_scrollPos, EditorStyles.helpBox).y;
            for (int i = 0; i < _names.Count; i++)
            {
                EditorGUILayout.LabelField(_names[i], GUILayout.MaxWidth(150f), GUILayout.MinWidth(10f));
            }
            GUILayout.EndScrollView();

            GUILayout.Space(10f);

            EditorGUILayout.LabelField("Add gamepad axes:", EditorStyles.boldLabel);
            GUILayout.Space(5f);

            EditorGUILayout.BeginHorizontal();

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


            if (GUILayout.Button("Add", GUILayout.MinWidth(50f), GUILayout.Height(30f)))
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
                SaveInputManager();
            }

            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            if (EditorGuiLayout.CenterButton("Remove gamepad axes", GUILayout.Height(30f), GUILayout.Width(150f)))
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
                SaveInputManager();
            }

            GUILayout.Space(10f);
        }

        // -- //

        private SerializedProperty GetAxes()
        {
            return _inputManager.FindProperty("m_Axes");
        }

        private void RefreshAxes()
        {
            _names.Clear();

            SerializedProperty axis;

            for (int i = 0; i < _axesArray.arraySize; i++)
            {
                axis = _axesArray.GetArrayElementAtIndex(i);
                _names.Add(axis.FindPropertyRelative("m_Name").stringValue);
            }
        }

        private void AddAxis(int axNum, int padNum, StringBuilder builder)
        {
            SerializedProperty axis = _axesArray.PlaceArrayElement();

            axis.FindPropertyRelative("m_Name").stringValue = InputUnility.AxisName(axNum, padNum, builder);
            axis.FindPropertyRelative("dead").floatValue = 0.1f;
            axis.FindPropertyRelative("sensitivity").floatValue = 1f;
            axis.FindPropertyRelative("type").enumValueIndex = 2;
            axis.FindPropertyRelative("axis").enumValueIndex = axNum - 1;
            axis.FindPropertyRelative("joyNum").enumValueIndex = padNum;
        }

        private void SaveInputManager()
        {
            _inputManager.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }
    }
}
