using UnityObject = UnityEngine.Object;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UU.Controls.ControlStuff;
using System.Text;

namespace UUEditor.Windows
{
    internal class AxisCreateWindow : EditorWindow
    {
        private SerializedObject m_inputManager;
        private SerializedProperty m_axesArray;
        private List<string> m_names;

        private Vector2 m_scrollPos;

        private int m_pads = 2;
        private int m_axes = 15;

        private void Awake()
        {
            minSize = new Vector2(300f, 300f);
            maxSize = new Vector2(300f, 1500f);

            UnityObject asset = AssetDatabase.LoadAssetAtPath<UnityObject>("ProjectSettings/InputManager.asset");
            m_inputManager = new SerializedObject(asset);
            m_axesArray = f_getAxes();
            m_names = new List<string>();
            f_refreshAxes();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            m_scrollPos.y = GUILayout.BeginScrollView(m_scrollPos, false, false).y;
            for (int i = 0; i < m_names.Count; i++)
            {
                EditorGUILayout.LabelField(m_names[i], GUILayout.MaxWidth(150f), GUILayout.MinWidth(10f));
            }
            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);


            EditorGUILayout.LabelField("Add gamepad axes:", EditorStyles.boldLabel);
            GUILayout.Space(5f);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Gamepads amount:", GUILayout.MaxWidth(150f), GUILayout.MinWidth(10f));
            m_pads = EditorGUILayout.IntField(m_pads, GUILayout.Width(30f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Axes amount:", GUILayout.MaxWidth(150f), GUILayout.MinWidth(10f));
            m_axes = EditorGUILayout.IntField(m_axes, GUILayout.Width(30f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();


            if (GUILayout.Button("Add", GUILayout.MinWidth(50f), GUILayout.Height(30f)))
            {
                StringBuilder builder = new StringBuilder();

                for (int pad = 0; pad < m_pads; pad++)
                {
                    for (int axis = 0; axis < m_axes; axis++)
                    {
                        f_addAxis(axis + 1, pad + 1, builder);
                    }
                }
                f_refreshAxes();
                f_saveInputManager();
            }

            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            if (EditorScriptUtility.DrawCenterButton("Remove gamepad axes", 100f, 30f))
            {
                int i = 0;

                while (i < m_axesArray.arraySize)
                {
                    SerializedProperty axis = m_axesArray.GetArrayElementAtIndex(i);

                    if (axis.FindPropertyRelative("type").enumValueIndex != 2)
                    {
                        i++;
                        continue;
                    }

                    string name = axis.FindPropertyRelative("m_Name").stringValue;
                    int num = axis.FindPropertyRelative("joyNum").enumValueIndex;
                    bool propVal = name[0] - (char)num == '@' && name[1] == ':' && int.TryParse(name[2].ToString(), out _);

                    if (!propVal) { i++; }
                    else { m_axesArray.DeleteArrayElementAtIndex(i); }
                }

                f_refreshAxes();
                f_saveInputManager();
            }

            GUILayout.Space(10f);
        }

        // -- //

        private SerializedProperty f_getAxes()
        {
            return m_inputManager.FindProperty("m_Axes");
        }

        private void f_refreshAxes()
        {
            m_names.Clear();

            SerializedProperty axis;

            for (int i = 0; i < m_axesArray.arraySize; i++)
            {
                axis = m_axesArray.GetArrayElementAtIndex(i);
                m_names.Add(axis.FindPropertyRelative("m_Name").stringValue);
            }
        }

        private void f_addAxis(int axNum, int padNum, StringBuilder builder)
        {
            m_axesArray.InsertArrayElementAtIndex(m_axesArray.arraySize);

            SerializedProperty axis = m_axesArray.GetArrayElementAtIndex(m_axesArray.arraySize - 1);

            axis.FindPropertyRelative("m_Name").stringValue = InputUnility.AxisName(axNum, padNum, builder);
            axis.FindPropertyRelative("dead").floatValue = 0.1f;
            axis.FindPropertyRelative("sensitivity").floatValue = 1f;
            axis.FindPropertyRelative("type").enumValueIndex = 2;
            axis.FindPropertyRelative("axis").enumValueIndex = axNum - 1;
            axis.FindPropertyRelative("joyNum").enumValueIndex = padNum;
        }

        private void f_saveInputManager()
        {
            m_inputManager.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }
    }
}
