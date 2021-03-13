using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;
using UnityUtility.NodeBased;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class TransitionViewer
    {
        private const float LINE_THICKNESS = 2.5f;

        private GraphEditorWindow _window;

        private SerializedProperty _transitionProp;
        private SerializedProperty _pointsProp;

        private PortViewer _in;
        private PortViewer _out;
        private List<PointViewer> _points;

        private bool _controlPresed;

        public PortViewer In => _in;
        public PortViewer Out => _out;
        public int PointsCount => _points.Count;

        public TransitionViewer(PortViewer outPoint, PortViewer inPoint, SerializedProperty transitionProp, GraphEditorWindow window)
        {
            if (outPoint.Type == inPoint.Type)
                throw new ArgumentException("Connection point types cannot be equal.");

            _window = window;
            _out = outPoint;
            _in = inPoint;
            _points = new List<PointViewer>();
            _transitionProp = transitionProp;

            _pointsProp = transitionProp.FindPropertyRelative(Transition.PointsFieldName);
            foreach (SerializedProperty item in _pointsProp)
            {
                _points.Add(new PointViewer(item, this, window));
            }
        }

        public void AddPoint()
        {
            SerializedProperty pointProp = _pointsProp.PlaceArrayElement();

            if (_points.Count == 0)
                pointProp.vector2Value = (_out.Node.GetRectInWorld().center + _in.Node.GetRectInWorld().center) * 0.5f;
            else
                pointProp.vector2Value += Vector2.one * 30f;

            _points.Add(new PointViewer(pointProp, this, _window));

            GUI.changed = true;
        }

        public void RemovePoint()
        {
            _pointsProp.arraySize--;
            _points.Pop();
            GUI.changed = true;
        }

        public void ShowTransitionInfoWindow()
        {
            TransitionInfoWindow.Open(this, _transitionProp, _window);
        }

        public void Draw()
        {
            Vector2 outPoint = _out.ScreenRect.center;
            Vector2 inPoint = _in.ScreenRect.center;

            Color targetColor = GraphEditorStyles.GetLineColor();

            if (_points.Count == 0)
            {
                DrawLine(outPoint, inPoint, Vector2.right, Vector2.left, targetColor);
                DrawButon((outPoint + inPoint) * 0.5f, targetColor);
                return;
            }

            Vector2 prevPoint = outPoint;
            for (int i = 0; i < _points.Count; i++)
            {
                Vector2 nexpoint = _window.Camera.WorldToScreen(_points[i].Position);
                DrawLine(prevPoint, nexpoint, i == 0, false, targetColor);
                _points[i].Draw(targetColor);
                prevPoint = nexpoint;
            }

            DrawLine(prevPoint, inPoint, _points.Count == 0, true, targetColor);
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
            Handles.DrawBezier(start, end, start + startTangentDir * factor, end + endTangentDir * factor, color, null, LINE_THICKNESS);
        }

        private static void DrawLine(in Vector2 start, in Vector2 end, bool lockStartTangent, bool lockEndTangent, in Color color)
        {
            float factor = GetPointTangentFactor(start, end);
            Vector2 startTangentdir = lockStartTangent ? Vector2.right : new Vector2(end.x - start.x, 0f).normalized;
            Vector2 endTangentdir = lockEndTangent ? Vector2.left : new Vector2(start.x - end.x, 0f).normalized;

            Handles.DrawBezier(start, end, start + startTangentdir * factor, end + endTangentdir * factor, color, null, LINE_THICKNESS);
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
#endif
