using OlegHcp.Mathematics;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Window.NodeBased.NodeDrawing
{
    internal class ControlPointViewer
    {
        private TransitionViewer _transitionViewer;
        private Vector3[] _trianglePoints = new Vector3[4];
        private Rect _rect = new Rect(0f, 0f, 8f, 8f);

        public ControlPointViewer(TransitionViewer transitionViewer)
        {
            _transitionViewer = transitionViewer;
        }

        public void DrawSquare(in Vector2 position, in Color color)
        {
            CreateRect(position);
            Handles.DrawSolidRectangleWithOutline(_rect, color, default);
        }

        public void DrawTriangle(in Vector2 position, in Quaternion rotation, in Color color)
        {
            CreateRect(position);
            DrawTriangleInternal(position, rotation, 6f, color);
        }

        public bool HandleEvents(Event e)
        {
            if (e.type != EventType.MouseDown || e.button != 0)
                return false;

            if (!_rect.Contains(e.mousePosition))
                return false;

            if (e.control)
                _transitionViewer.Delete();
            else
                _transitionViewer.ShowInfoWindow();

            return true;
        }

        private void CreateRect(in Vector2 position)
        {
            _rect.x = position.x - _rect.width * 0.5f;
            _rect.y = position.y - _rect.height * 0.5f;
        }

        private void DrawTriangleInternal(in Vector3 position, in Quaternion rotation, float size, in Color color)
        {
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

            Handles.DrawSolidRectangleWithOutline(_trianglePoints, color, default);
        }
    }
}
