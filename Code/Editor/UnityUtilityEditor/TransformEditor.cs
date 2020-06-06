using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;

namespace UnityUtilityEditor
{
    [CustomEditor(typeof(Transform))]
    internal class TransformEditor : Editor
    {
        private const string UNDO_NAME = "Transform";
        private const float SPACE = 5f;
        private const float BTN_WIDTH = 35f;
        private const float LABEL_WIDTH = 35f;
        private const float TOGGLE_WIDTH = 15f;

        private static string[] s_toolbarNames = new string[] { "Local", "World" };
        private static Vector3[] s_axes = { Vector3.forward, Vector3.right, Vector3.up };

        private static bool s_world;
        private static bool s_grid;
        private static float s_snapStep = 1f;

        private Transform m_target;
        private Handles m_h;
        private SerializedProperty m_posProp;
        private SerializedProperty m_rotProp;
        private SerializedProperty m_sclProp;

        private void Awake()
        {
            m_target = target as Transform;
        }

        private void OnEnable()
        {
            m_posProp = serializedObject.FindProperty("m_LocalPosition");
            m_rotProp = serializedObject.FindProperty("m_LocalRotation");
            m_sclProp = serializedObject.FindProperty("m_LocalScale");
        }

        private void OnSceneGUI()
        {
            if (s_grid)
            {
                Tool curTool = UnityEditor.Tools.current;

                UnityEditor.Tools.hidden = curTool == Tool.Move || curTool == Tool.Transform || curTool == Tool.Rect;

                if (!UnityEditor.Tools.hidden)
                    return;

                if (curTool == Tool.Transform || curTool == Tool.Rect)
                {
                    Handles.Label(m_target.position, "Not allowed\nfor grid\nsnapping.");
                    return;
                }

                EditorGUI.BeginChangeCheck();

                Vector3 pos = Handles.PositionHandle(m_target.position, Quaternion.identity);

                pos.x = pos.x.Round(s_snapStep);
                pos.y = pos.y.Round(s_snapStep);
                pos.z = pos.z.Round(s_snapStep);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_target, UNDO_NAME);

                    m_target.position = pos;
                }

                const int SIZE = 7;

                Vector3 n = -(m_h ?? (m_h = new Handles())).currentCamera.transform.forward;
                const float CIRCLE_SIZE = 0.03f;

                int index = s_axes.IndexOfMax(itm => (Vector3.Angle(itm, n) - 90).Abs());

                Vector3 nodePos(int i, int j)
                {
                    float snapStepI = s_snapStep * (i - SIZE / 2);
                    float snapStepJ = s_snapStep * (j - SIZE / 2);

                    switch (index)
                    {
                        case 0: Handles.color = Color.blue; return new Vector3(pos.x + snapStepI, pos.y + snapStepJ, pos.z);
                        case 1: Handles.color = Color.red; return new Vector3(pos.x, pos.y + snapStepJ, pos.z + snapStepI);
                        case 2: Handles.color = Color.green; return new Vector3(pos.x + snapStepI, pos.y, pos.z + snapStepJ);

                        default: throw new UnsupportedValueException(index);
                    }
                }

                for (int i = 0; i < SIZE; i++)
                {
                    for (int j = 0; j < SIZE; j++)
                    {
                        Vector3 circlePos = nodePos(i, j);
                        Handles.DrawSolidDisc(circlePos, n, HandleUtility.GetHandleSize(Vector3.zero) * CIRCLE_SIZE);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(SPACE);

            bool world = s_world = GUILayout.Toolbar(s_world.ToInt(), s_toolbarNames).ToBool();

            GUILayout.Space(SPACE);

            if (world)
            {
                bool drawLine(string label, in Vector3 curValue, out Vector3 newValue, bool locked = false)
                {
                    bool changed;
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(label, GUILayout.Width(LABEL_WIDTH));
                    if (locked)
                    {
                        GUI.enabled = false;
                        EditorGUILayout.Vector3Field(GUIContent.none, curValue);
                        GUI.enabled = true;
                        changed = false;
                        newValue = default;
                    }
                    else
                    {
                        EditorGUI.BeginChangeCheck();
                        newValue = EditorGUILayout.Vector3Field(GUIContent.none, curValue);
                        if (changed = EditorGUI.EndChangeCheck())
                            Undo.RecordObject(m_target, UNDO_NAME);
                    }
                    EditorGUILayout.EndHorizontal();
                    return changed;
                }

                if (drawLine("[Pos]", m_target.position, out Vector3 pos)) { m_target.position = pos; }
                if (drawLine("[Rot]", m_target.eulerAngles, out Vector3 rot)) { m_target.eulerAngles = rot; }
                drawLine("[Scl]", m_target.lossyScale, out _, true);
            }
            else
            {
                bool changed = false;

                bool drawLine(string label, in Vector3 curValue, out Vector3 newValue, in Vector3 defVal = default)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(label, GUILayout.Width(BTN_WIDTH))) { newValue = defVal; }
                    else { newValue = EditorGUILayout.Vector3Field(GUIContent.none, curValue); }
                    EditorGUILayout.EndHorizontal();
                    bool ch = EditorGUI.EndChangeCheck();
                    changed |= ch;
                    return ch;
                }

                if (drawLine("Pos", m_posProp.vector3Value, out Vector3 pos)) { m_posProp.vector3Value = pos; }
                if (drawLine("Rot", m_rotProp.quaternionValue.eulerAngles, out Vector3 rot)) { m_rotProp.quaternionValue = rot.ToRotation(); }
                if (drawLine("Scl", m_sclProp.vector3Value, out Vector3 scl, Vector3.one)) { m_sclProp.vector3Value = scl; }

                if (changed)
                    serializedObject.ApplyModifiedProperties();
            }

            GUILayout.Space(SPACE);

            EditorGUILayout.BeginHorizontal();
            bool gridNewVal = GUILayout.Toggle(s_grid, string.Empty, GUILayout.Width(TOGGLE_WIDTH));
            if (s_grid != gridNewVal)
            {
                s_grid = gridNewVal;
                UnityEditor.Tools.hidden = gridNewVal;
                EditorWindow.GetWindow<SceneView>().Repaint();
            }
            EditorGUI.BeginDisabledGroup(!s_grid);
            s_snapStep = EditorGUILayout.FloatField("Grid Snap Step", s_snapStep);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(SPACE);
        }
    }
}
