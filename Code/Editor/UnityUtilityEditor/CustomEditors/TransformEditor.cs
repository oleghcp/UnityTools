using UnityEditor;
using UnityEngine;
using UnityUtility.MathExt;

#if !UNITY_2019_3_OR_NEWER
using System.Collections.Generic;
using UnityUtility; 
#endif

namespace UnityUtilityEditor.CustomEditors
{
    [CustomEditor(typeof(Transform))]
    internal class TransformEditor : Editor<Transform>
    {
        private const string UNDO_NAME = "Transform";
        private const float SPACE = 5f;
        private const float BTN_WIDTH = 40f;
        private const float LABEL_WIDTH = 40f;

        private static readonly string[] _toolbarNames = new string[] { "Local", "World" };

        private static bool _world;

        private SerializedProperty _posProp;
        private SerializedProperty _rotProp;
        private SerializedProperty _sclProp;

#if !UNITY_2019_3_OR_NEWER
        private const float TOGGLE_WIDTH = 15f;
        private static bool _grid;
        private static float _snapStep = 1f;
        private readonly static Vector3[] s_axes = { Vector3.forward, Vector3.right, Vector3.up };
        private Handles _handles;
#endif

        private void OnEnable()
        {
            _posProp = serializedObject.FindProperty("m_LocalPosition");
            _rotProp = serializedObject.FindProperty("m_LocalRotation");
            _sclProp = serializedObject.FindProperty("m_LocalScale");
        }

#if !UNITY_2019_3_OR_NEWER
        private void OnSceneGUI()
        {
            if (_grid)
            {
                Tool curTool = Tools.current;

                Tools.hidden = curTool == Tool.Move || curTool == Tool.Transform || curTool == Tool.Rect;

                if (!Tools.hidden)
                    return;

                if (curTool == Tool.Transform || curTool == Tool.Rect)
                {
                    Handles.Label(target.position, "Not allowed\nfor grid\nsnapping.");
                    return;
                }

                EditorGUI.BeginChangeCheck();

                Vector3 pos = Handles.PositionHandle(target.position, Quaternion.identity);

                pos.x = pos.x.Round(_snapStep);
                pos.y = pos.y.Round(_snapStep);
                pos.z = pos.z.Round(_snapStep);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, UNDO_NAME);

                    target.position = pos;
                }

                const int SIZE = 7;

                Vector3 n = -(_handles ?? (_handles = new Handles())).currentCamera.transform.forward;
                const float CIRCLE_SIZE = 0.03f;

                int index = s_axes.IndexOfMax(itm => (Vector3.Angle(itm, n) - 90).Abs());

                Vector3 nodePos(int i, int j)
                {
                    float snapStepI = _snapStep * (i - SIZE / 2);
                    float snapStepJ = _snapStep * (j - SIZE / 2);

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
#endif

        public override void OnInspectorGUI()
        {
            bool hasParent = target.parent;

            if (!hasParent)
                _world = false;

            GUILayout.Space(SPACE);

            GUI.enabled = hasParent;
            _world = GUILayout.Toolbar(_world.ToInt(), _toolbarNames)
                               .ToBool();
            GUI.enabled = true;

            GUILayout.Space(SPACE);

            if (_world)
            {
                bool drawLine(string label, in Vector3 curValue, out Vector3 newValue, bool locked = false)
                {
                    bool changed;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(label, GUILayout.Width(LABEL_WIDTH));
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
                            Undo.RecordObject(target, UNDO_NAME);
                    }
                    EditorGUILayout.EndHorizontal();
                    return changed;
                }

                if (drawLine("[ Pos ]", target.position, out Vector3 pos)) { target.position = pos; }
                if (drawLine("[ Rot ]", target.eulerAngles, out Vector3 rot)) { target.eulerAngles = rot; }
                drawLine("[ Scl ]", target.lossyScale, out _, true);
            }
            else
            {
                bool changed = false;

                bool drawLine(string label, in Vector3 curValue, out Vector3 newValue, in Vector3 defVal = default)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(label, GUILayout.Width(BTN_WIDTH), GUILayout.Height(EditorGUIUtility.singleLineHeight))) { newValue = defVal; }
                    else { newValue = EditorGUILayout.Vector3Field(GUIContent.none, curValue); }
                    EditorGUILayout.EndHorizontal();
                    bool ch = EditorGUI.EndChangeCheck();
                    changed |= ch;
                    return ch;
                }

                if (drawLine("Pos", _posProp.vector3Value, out Vector3 pos)) { _posProp.vector3Value = pos; }
                if (drawLine("Rot", _rotProp.quaternionValue.eulerAngles, out Vector3 rot)) { _rotProp.quaternionValue = rot.ToRotation(); }
                if (drawLine("Scl", _sclProp.vector3Value, out Vector3 scl, Vector3.one)) { _sclProp.vector3Value = scl; }

                if (changed)
                    serializedObject.ApplyModifiedProperties();
            }

            GUILayout.Space(SPACE);

#if !UNITY_2019_3_OR_NEWER
            EditorGUILayout.BeginHorizontal();
            bool gridNewVal = GUILayout.Toggle(_grid, string.Empty, GUILayout.Width(TOGGLE_WIDTH));
            if (_grid != gridNewVal)
            {
                _grid = gridNewVal;
                Tools.hidden = gridNewVal;
                EditorWindow.GetWindow<SceneView>()
                            .Repaint();
            }
            EditorGUI.BeginDisabledGroup(!_grid);
            _snapStep = EditorGUILayout.FloatField("Grid Snap Step", _snapStep);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(SPACE);
#endif
        }
    }
}
