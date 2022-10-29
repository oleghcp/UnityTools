#if UNITY_2019_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;
using UnityUtility.NodeBased;

namespace UnityUtilityEditor.Window.NodeBased.NodeDrawing
{
    internal enum TransitionViewType
    {
        Splines,
        Arrows,
    }

    internal class TransitionViewer
    {
        private const float LINE_THICKNESS = 2.5f;

        private GraphEditorWindow _window;

        private PortViewer _source;
        private PortViewer _destination;
        private List<PointViewer> _points;

        private bool _controlPresed;
        private Vector3[] _trianglePoints = new Vector3[4];
        private Vector3[] _linePoints = new Vector3[2];

        public PortViewer Source => _source;
        public PortViewer Destination => _destination;
        public int PointsCount => _points.Count;

        public TransitionViewer(PortViewer source, PortViewer destination, GraphEditorWindow window)
        {
            if (source.Type == destination.Type)
                throw new ArgumentException("Connection point types cannot be equal.");

            _window = window;
            _source = source;
            _destination = destination;
            _points = new List<PointViewer>();

            SerializedProperty pointsProp = GetProperty().FindPropertyRelative(Transition.PointsFieldName);
            foreach (SerializedProperty item in pointsProp)
            {
                _points.Add(new PointViewer(item.vector2Value, this, window));
            }
        }

        public void AddPoint()
        {
            SerializedProperty pointsProp = GetProperty().FindPropertyRelative(Transition.PointsFieldName);
            SerializedProperty pointProp = pointsProp.AddArrayElement();

            if (_points.Count == 0)
                pointProp.vector2Value = (_source.Node.WorldRect.center + _destination.Node.WorldRect.center) * 0.5f;
            else
                pointProp.vector2Value += Vector2.one * 30f;

            _points.Add(new PointViewer(pointProp.vector2Value, this, _window));

            GUI.changed = true;
        }

        public void RemovePoint()
        {
            SerializedProperty pointsProp = GetProperty().FindPropertyRelative(Transition.PointsFieldName);
            pointsProp.arraySize--;
            _points.Pop();
            GUI.changed = true;
        }

        public void ShowTransitionInfoWindow()
        {
            TransitionInfoPopup.Open(this, GetProperty(), _window);
        }

        public void Save()
        {
            SerializedProperty pointsProp = GetProperty().FindPropertyRelative(Transition.PointsFieldName);

            for (int i = 0; i < _points.Count; i++)
            {
                pointsProp.GetArrayElementAtIndex(i).vector2Value = _points[i].Position;
            }
        }

        public void DrawSpline()
        {
            if (!(_source.Node.IsInCamera || _destination.Node.IsInCamera))
                return;

            Color targetColor = GraphEditorStyles.GetLineColor();

            Vector2 sourcePoint = _source.ScreenRect.center;
            Vector2 destPoint = _destination.ScreenRect.center;

            if (_points.Count == 0)
            {
                DrawSpline(sourcePoint, destPoint, Vector2.right, Vector2.left, targetColor);
                DrawDotButon((sourcePoint + destPoint) * 0.5f, targetColor);
                return;
            }

            Vector2 prevPoint = sourcePoint;
            Vector2 prevEndTangentDir = default;

            for (int i = 0; i < _points.Count; i++)
            {
                Vector2 nexpoint = _window.Camera.WorldToScreen(_points[i].Position);

                float startTangentFactor = i == 0 ? GetTangentFactor(prevPoint, nexpoint)
                                                  : GetPointTangentFactor(prevPoint, nexpoint);
                float endTangentFactor = GetPointTangentFactor(prevPoint, nexpoint);

                Vector2 startTangentDir = i == 0 ? Vector2.right : -prevEndTangentDir;
                prevEndTangentDir = new Vector2(prevPoint.x - nexpoint.x, 0f).normalized;

                DrawSpline(prevPoint, nexpoint, startTangentFactor, endTangentFactor, startTangentDir, prevEndTangentDir, targetColor);
                _points[i].Draw(targetColor);

                prevPoint = nexpoint;
            }

            DrawSpline(prevPoint, destPoint, GetPointTangentFactor(prevPoint, destPoint), GetTangentFactor(prevPoint, destPoint), -prevEndTangentDir, Vector2.left, targetColor);
        }

        public void DrawArrow()
        {
            if (!(_source.Node.IsInCamera || _destination.Node.IsInCamera))
                return;

            Color targetColor = GraphEditorStyles.GetLineColor();

            Vector2 sourcePoint = _source.Node.ScreenRect.center;
            Vector2 destPoint = _destination.Node.ScreenRect.center;

            Vector2 vector = destPoint - sourcePoint;
            Vector2 shift = new Vector2(vector.y, -vector.x).normalized * 5f;

            _linePoints[0] = sourcePoint + shift;
            _linePoints[1] = destPoint + shift;

            Handles.DrawAAPolyLine(LINE_THICKNESS, _linePoints);
            Vector2 pos = (_linePoints[0] + _linePoints[1]) * 0.5f;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, vector);
            DrawArrowButon(pos, rot, targetColor);
        }

        public bool ProcessEvents(Event e)
        {
            _controlPresed = e.control;

            bool needLock = false;

            for (int i = 0; i < _points.Count; i++)
            {
                if (_points[i].ProcessEvents(e))
                    needLock = true;
            }

            return needLock;
        }

        private void DrawDotButon(in Vector2 position, in Color color)
        {
            Handles.color = color;
            if (Handles.Button(position, Quaternion.identity, 4f, 8f, Handles.DotHandleCap))
                ButtonAction();
            Handles.color = Colours.White;
        }

        private void DrawArrowButon(in Vector2 position, in Quaternion rotation, in Color color)
        {
            Handles.color = color;
            if (Handles.Button(position, rotation, 6f, 8f, TriangleHandleCap))
                ButtonAction();
            Handles.color = Colours.White;
        }

        private void ButtonAction()
        {
            if (Event.current.button != 0)
                return;

            if (_controlPresed)
                _source.Node.RemoveTransition(this);
            else
                ShowTransitionInfoWindow();
        }

        public static void DrawSpline(in Vector2 start, in Vector2 end, in Vector2 startTangentDir, in Vector2 endTangentDir)
        {
            DrawSpline(start, end, startTangentDir, endTangentDir, GraphEditorStyles.GetLineColor());
        }

        public static void DrawSpline(in Vector2 start, in Vector2 end, in Vector2 startTangentDir, in Vector2 endTangentDir, in Color color)
        {
            float factor = GetTangentFactor(start, end);
            DrawSpline(start, end, factor, factor, startTangentDir, endTangentDir, color);
        }

        private static void DrawSpline(in Vector2 start, in Vector2 end,
                                       float startTangentFactor, float endTangentFactor,
                                       in Vector2 startTangentdir, in Vector2 endTangentdir,
                                       in Color color)
        {
            Handles.DrawBezier(start, end, start + startTangentdir * startTangentFactor, end + endTangentdir * endTangentFactor, color, null, LINE_THICKNESS);
        }

        public static void DrawDirection(in Vector2 start, in Vector2 end)
        {
            Handles.DrawAAPolyLine(LINE_THICKNESS, start, end);
        }

        // -- //

        private SerializedProperty GetProperty()
        {
            SerializedProperty nodeProp = _source.Node.NodeProp;
            return nodeProp.FindPropertyRelative(RawNode.ArrayFieldName)
                           .EnumerateArrayElements()
                           .First(item => item.FindPropertyRelative(Transition.NodeIdFieldName).intValue == _destination.Node.Id);
        }

        private static float GetTangentFactor(in Vector2 start, in Vector2 end)
        {
            float x = (end.x - start.x).Abs();
            float y = (end.y - start.y).Abs().ClampMax(x);
            return (x + y) * 0.3f;
        }

        private static float GetPointTangentFactor(in Vector2 start, in Vector2 end)
        {
            float x = (end.x - start.x).Abs();
            float y = (end.y - start.y).Abs();
            return (x + y) * 0.3f;
        }

        private void TriangleHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.MouseMove:
                case EventType.Layout:
                    HandleUtility.AddControl(controlID, HandleUtility.DistanceToRectangle(position, rotation, size));
                    break;

                case EventType.Repaint:
                    Vector2 pos = position;
                    float angle = rotation.eulerAngles.z + 90f;

                    Vector2 vector0 = MathUtility.AngleToVector2(angle) * size;
                    Vector2 vector1 = MathUtility.AngleToVector2(angle + 120f) * size;
                    Vector2 vector2 = MathUtility.AngleToVector2(angle - 120f) * size;
                    Vector2 lookVector = pos + vector0;

                    _trianglePoints[0] = pos + vector1;
                    _trianglePoints[1] = lookVector;
                    _trianglePoints[2] = lookVector;
                    _trianglePoints[3] = pos + vector2;

                    Handles.DrawSolidRectangleWithOutline(_trianglePoints, Colours.White, default);
                    break;
            }
        }
    }
}
#endif
