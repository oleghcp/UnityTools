using System;
using System.Collections.Generic;
using System.Linq;
using OlegHcp.CSharp.Collections;
using OlegHcp.Mathematics;
using OlegHcp.NodeBased.Service;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Window.NodeBased.NodeDrawing
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
        private ControlPointViewer _controlPointViewer;
        private List<PointViewer> _points;
        private Vector3[] _linePoints = new Vector3[2];

        public PortViewer Source => _source;
        public PortViewer Destination => _destination;
        public int PointsCount => _points == null ? 0 : _points.Count;

        public TransitionViewer(PortViewer source, PortViewer destination, GraphEditorWindow window)
        {
            if (source.Type == destination.Type)
                throw new ArgumentException("Connection point types cannot be equal.");

            _window = window;
            _source = source;
            _destination = destination;
            _controlPointViewer = new ControlPointViewer(this);

            SerializedProperty pointsProp = GetProperty().FindPropertyRelative(Transition.PointsFieldName);

            if (pointsProp.arraySize > 0)
            {
                _points = new List<PointViewer>();
                foreach (SerializedProperty item in pointsProp.EnumerateArrayElements())
                {
                    _points.Add(new PointViewer(item.vector2Value, this, window));
                }
            }
        }

        public void AddPoint()
        {
            SerializedProperty pointsProp = GetProperty().FindPropertyRelative(Transition.PointsFieldName);
            SerializedProperty pointProp = pointsProp.AddArrayElement();

            if (_points == null)
                _points = new List<PointViewer>();

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

        public void ShowInfoWindow()
        {
            TransitionInfoPopup.Open(this, GetProperty(), _window);
        }

        public void Delete()
        {
            _source.Node.RemoveTransition(this);
        }

        public void Save()
        {
            if (_points.IsNullOrEmpty())
                return;

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

            if (_points.IsNullOrEmpty())
            {
                DrawSpline(sourcePoint, destPoint, Vector2.right, Vector2.left, targetColor);
                _controlPointViewer.DrawSquare((sourcePoint + destPoint) * 0.5f, targetColor);
                return;
            }

            Vector2 prevPoint = sourcePoint;
            Vector2 prevEndTangentDir = default;

            for (int i = 0; i < _points.Count; i++)
            {
                Vector2 nextPoint = _window.Camera.WorldToScreen(_points[i].Position);

                float startTangentFactor = i == 0 ? GetTangentFactor(prevPoint, nextPoint)
                                                  : GetPointTangentFactor(prevPoint, nextPoint);
                float endTangentFactor = GetPointTangentFactor(prevPoint, nextPoint);

                Vector2 startTangentDir = i == 0 ? Vector2.right : -prevEndTangentDir;
                prevEndTangentDir = new Vector2(prevPoint.x - nextPoint.x, 0f).normalized;

                DrawSpline(prevPoint, nextPoint, startTangentFactor, endTangentFactor, startTangentDir, prevEndTangentDir, targetColor);
                _points[i].Draw(targetColor);

                prevPoint = nextPoint;
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
            _controlPointViewer.DrawTriangle(pos, rot, targetColor);
        }

        public bool HandleEvents(Event e, bool showPoints)
        {
            if (!showPoints || _points.IsNullOrEmpty())
                return _controlPointViewer.HandleEvents(e);

            bool needLock = false;

            for (int i = 0; i < _points.Count; i++)
            {
                if (_points[i].HandleEvents(e))
                    needLock = true;
            }

            return needLock;
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
    }
}
