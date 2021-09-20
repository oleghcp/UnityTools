using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;

#if !UNITY_2019_3_OR_NEWER
using System.Collections.Generic;
#endif

namespace UnityUtilityEditor.Inspectors
{
    [CustomEditor(typeof(Transform))]
    internal class TransformEditor : Editor<Transform>
    {
        private const string UNDO_NAME = "Transform";
        private const float SPACE = 5f;
        private const float BTN_WIDTH = 40f;
        private const float LABEL_WIDTH = 40f;

        private string[] _toolbarNames = new string[] { "Local", "World" };
        private string _pivotModeWarning = $"→ {PivotMode.Center}";
        private string _pivotRotationWarning = $"→ {PivotRotation.Global}";
        private Rect _sceneGuiArea = new Rect(5f, 0f, 60f, 40f);

        private static bool _world;

        private SerializedProperty _posProp;
        private SerializedProperty _rotProp;
        private SerializedProperty _sclProp;

#if !UNITY_2019_3_OR_NEWER
        private static bool _grid;
        private static float _snapStep = 1f;
        private Vector3[] _axes = { Vector3.forward, Vector3.right, Vector3.up };
        private Handles _handles;
#endif

        private void OnEnable()
        {
            _posProp = serializedObject.FindProperty("m_LocalPosition");
            _rotProp = serializedObject.FindProperty("m_LocalRotation");
            _sclProp = serializedObject.FindProperty("m_LocalScale");

#if UNITY_2021_1_OR_NEWER
            Tools.pivotModeChanged += SceneView.RepaintAll;
            Tools.pivotRotationChanged += SceneView.RepaintAll;
#endif
            SceneView.beforeSceneGui += DarwSceneGUI;
        }

        private void OnDisable()
        {
#if UNITY_2021_1_OR_NEWER
            Tools.pivotModeChanged -= SceneView.RepaintAll;
            Tools.pivotRotationChanged -= SceneView.RepaintAll;
#endif
            SceneView.beforeSceneGui -= DarwSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(SPACE);

            bool hasParent = target.parent;

            GUI.enabled = hasParent;
            _world = GUILayout.Toolbar((hasParent && _world).ToInt(), _toolbarNames).ToBool();
            GUI.enabled = true;

            GUILayout.Space(SPACE);

            if (_world)
            {
                if (DrawLineForWorld("[ Pos ]", target.position, out Vector3 pos))
                    target.position = pos;

                if (DrawLineForWorld("[ Rot ]", target.eulerAngles, out Vector3 rot))
                    target.eulerAngles = rot;

                DrawLineForWorld("[ Scl ]", target.lossyScale, out _, true);
            }
            else
            {
                serializedObject.Update();

                _posProp.vector3Value = DrawLineForLocal("Pos", _posProp.vector3Value);
                _rotProp.quaternionValue = DrawLineForLocal("Rot", _rotProp.quaternionValue.eulerAngles).ToRotation();
                _sclProp.vector3Value = DrawLineForLocal("Scl", _sclProp.vector3Value, Vector3.one);

                serializedObject.ApplyModifiedProperties();
            }

            GUILayout.Space(SPACE);

#if !UNITY_2019_3_OR_NEWER
            EditorGUILayout.BeginHorizontal();
            bool gridNewVal = GUILayout.Toggle(_grid, "Grid Snap Step", GUI.skin.button,
                                               GUILayout.Width(EditorGUIUtility.labelWidth),
                                               GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (_grid != gridNewVal)
            {
                Tools.hidden = _grid = gridNewVal;
                SceneView.RepaintAll();
            }

            GUI.enabled = _grid;
            _snapStep = EditorGUILayout.DelayedFloatField(_snapStep);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(SPACE);
#endif
        }

        private void DarwSceneGUI(SceneView sceneView)
        {
            if (sceneView.in2DMode)
                return;

            GUILayout.BeginArea(_sceneGuiArea);

            if (Tools.pivotMode != PivotMode.Pivot)
            {
                GUI.color = Colours.Cyan;
                EditorGUILayout.HelpBox(_pivotModeWarning, MessageType.None);
            }

            if (Tools.pivotRotation != PivotRotation.Local)
            {
                GUI.color = Colours.Yellow;
                EditorGUILayout.HelpBox(_pivotRotationWarning, MessageType.None);
            }

            GUI.color = Colours.White;
            GUILayout.EndArea();
        }

#if !UNITY_2019_3_OR_NEWER
        private void OnSceneGUI()
        {
            if (!_grid)
                return;

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

            int index = _axes.IndexOfMax(itm => (Vector3.Angle(itm, n) - 90).Abs());

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
#endif

        private static Vector3 DrawLineForLocal(string label, in Vector3 curValue, in Vector3 defVal = default)
        {
            Vector3 newValue;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(label, GUILayout.Width(BTN_WIDTH), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                EditorGUILayout.Vector3Field(GUIContent.none, newValue = defVal);
            else
                newValue = EditorGUILayout.Vector3Field(GUIContent.none, curValue);
            EditorGUILayout.EndHorizontal();

            return newValue;
        }

        private bool DrawLineForWorld(string label, in Vector3 curValue, out Vector3 newValue, bool locked = false)
        {
            bool changed = false;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(LABEL_WIDTH));
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
}
