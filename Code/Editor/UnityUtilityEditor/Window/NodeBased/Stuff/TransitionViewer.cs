using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;
using UnityUtility.NodeBased;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class TransitionViewer
    {
        private const float LINE_THICKNESS = 2.5f;

        private GraphEditorWindow _window;

        private PortViewer _source;
        private PortViewer _destination;
        private List<PointViewer> _points;

        private bool _controlPresed;

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
            SerializedProperty pointProp = pointsProp.PlaceArrayElement();

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
            TransitionInfoWindow.Open(this, GetProperty(), _window);
        }

        public void Save()
        {
            SerializedProperty pointsProp = GetProperty().FindPropertyRelative(Transition.PointsFieldName);

            for (int i = 0; i < _points.Count; i++)
            {
                pointsProp.GetArrayElementAtIndex(i).vector2Value = _points[i].Position;
            }
        }

        public void Draw()
        {
            Color targetColor = GraphEditorStyles.GetLineColor();

            Vector2 outPoint = _source.ScreenRect.center;
            Vector2 inPoint = _destination.ScreenRect.center;

            if (_points.Count == 0)
            {
                DrawLine(outPoint, inPoint, Vector2.right, Vector2.left, targetColor);
                DrawButon((outPoint + inPoint) * 0.5f, targetColor);
                return;
            }

            Vector2 prevPoint = outPoint;
            Vector2 prevEndTangentDir = default;

            for (int i = 0; i < _points.Count; i++)
            {
                Vector2 nexpoint = _window.Camera.WorldToScreen(_points[i].Position);

                float startTangentFactor = i == 0 ? GetTangentFactor(prevPoint, nexpoint)
                                                  : GetPointTangentFactor(prevPoint, nexpoint);
                float endTangentFactor = GetPointTangentFactor(prevPoint, nexpoint);

                Vector2 startTangentDir = i == 0 ? Vector2.right : -prevEndTangentDir;
                prevEndTangentDir = new Vector2(prevPoint.x - nexpoint.x, 0f).normalized;

                DrawLine(prevPoint, nexpoint, startTangentFactor, endTangentFactor, startTangentDir, prevEndTangentDir, targetColor);
                _points[i].Draw(targetColor);

                prevPoint = nexpoint;
            }

            DrawLine(prevPoint, inPoint, GetPointTangentFactor(prevPoint, inPoint), GetTangentFactor(prevPoint, inPoint), -prevEndTangentDir, Vector2.left, targetColor);
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

        private void DrawButon(in Vector2 position, in Color color)
        {
            Handles.color = color;
            if (Handles.Button(position, Quaternion.identity, 4f, 8f, Handles.DotHandleCap))
            {
                if (_controlPresed)
                    _window.DeleteTransition(this);
                else
                    ShowTransitionInfoWindow();
            }
            Handles.color = Colours.White;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine(in Vector2 start, in Vector2 end, in Vector2 startTangentDir, in Vector2 endTangentDir)
        {
            DrawLine(start, end, startTangentDir, endTangentDir, GraphEditorStyles.GetLineColor());
        }

        public static void DrawLine(in Vector2 start, in Vector2 end, in Vector2 startTangentDir, in Vector2 endTangentDir, in Color color)
        {
            float factor = GetTangentFactor(start, end);
            DrawLine(start, end, factor, factor, startTangentDir, endTangentDir, color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawLine(in Vector2 start, in Vector2 end,
                                     float startTangentFactor, float endTangentFactor,
                                     in Vector2 startTangentdir, in Vector2 endTangentdir,
                                     in Color color)
        {
            Handles.DrawBezier(start, end, start + startTangentdir * startTangentFactor, end + endTangentdir * endTangentFactor, color, null, LINE_THICKNESS);
        }

        // -- //

        private SerializedProperty GetProperty()
        {
            return _source.Node
                          .FindSubProperty(RawNode.ArrayFieldName)
                          .EnumerateArrayElements()
                          .First(item => item.FindPropertyRelative(Transition.NodeIdFieldName).intValue == _destination.Node.Id);
        }

        private static float GetTangentFactor(in Vector2 start, in Vector2 end)
        {
            float x = (end.x - start.x).Abs();
            float y = (end.y - start.y).Abs().CutAfter(x);
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
