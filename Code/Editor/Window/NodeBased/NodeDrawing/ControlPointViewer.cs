using OlegHcp;
using OlegHcp.Mathematics;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Window.NodeBased.NodeDrawing
{
    internal class ControlPointViewer
    {
        private bool _controlPressed;
        private TransitionViewer _transitionViewer;
        private Vector3[] _trianglePoints = new Vector3[4];

        public ControlPointViewer(TransitionViewer transitionViewer)
        {
            _transitionViewer = transitionViewer;
        }

        public void DrawSquare(in Vector2 position, in Color color)
        {
            Handles.color = color;
            if (Handles.Button(position, Quaternion.identity, 4f, 8f, Handles.DotHandleCap))
                ButtonAction();
            Handles.color = Colours.White;
        }

        public void DrawTriangle(in Vector2 position, in Quaternion rotation, in Color color)
        {
            Handles.color = color;
            if (Handles.Button(position, rotation, 6f, 8f, TriangleHandleCap))
                ButtonAction();
            Handles.color = Colours.White;
        }

        public bool HandleEvents(Event e)
        {
            _controlPressed = e.control;

            return false;
        }

        private void ButtonAction()
        {
            if (Event.current.button != 0)
                return;

            if (_controlPressed)
                _transitionViewer.Delete();
            else
                _transitionViewer.ShowInfoWindow();
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
