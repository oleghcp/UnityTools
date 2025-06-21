using OlegHcp;
using OlegHcp.Mathematics;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Window.NodeBased.NodeDrawing
{
    internal class PointViewer
    {
        private const float VIEW_RADIUS = 5f;
        private const float PICK_SIZE = VIEW_RADIUS * 4f;

        private TransitionViewer _transitionViewer;
        private GraphEditorWindow _window;
        private bool _isDragged;
        private Vector2 _draggedPosition;
        private Vector2 _position;

        public Vector2 Position => _position;

        public PointViewer(Vector2 position, TransitionViewer transitionViewer, GraphEditorWindow window)
        {
            _position = position;
            _transitionViewer = transitionViewer;
            _window = window;
        }

        public void Draw(in Color color)
        {
            Handles.color = color;
            Handles.DrawSolidDisc(_window.Camera.WorldToScreen(Position), Vector3.back, VIEW_RADIUS);
            Handles.color = Colours.White;
        }

        public bool HandleEvents(Event e)
        {
            bool needLock = false;

            switch (e.type)
            {
                case EventType.MouseDown:
                    Vector2 rectSize = new Vector2(PICK_SIZE, PICK_SIZE);
                    Vector2 rectPos = _window.Camera.WorldToScreen(Position) - rectSize * 0.5f;
                    Rect pointRect = new Rect(rectPos, rectSize);

                    if (e.button == 0)
                    {
                        if (pointRect.Contains(e.mousePosition))
                        {
                            if (e.control)
                            {
                                _draggedPosition = Position;
                                _isDragged = e.control;
                                needLock = true;
                            }
                            else
                            {
                                _transitionViewer.ShowTransitionInfoWindow();
                            }
                        }
                        else
                        {
                            _isDragged = false;
                        }
                    }

                    break;

                case EventType.MouseUp:
                    _isDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && _isDragged && e.control)
                    {
                        Drag(e.delta);
                        GUI.changed = true;
                        needLock = true;
                    }
                    break;
            }

            return needLock;
        }

        private void Drag(Vector2 mouseDelta)
        {
            if (_window.GridSnapping)
            {
                _draggedPosition += mouseDelta * _window.Camera.Size;
                _position = new Vector2(_draggedPosition.x.Round(GraphGrid.SMALL_STEP), _draggedPosition.y.Round(GraphGrid.SMALL_STEP));
            }
            else
            {
                _position += mouseDelta * _window.Camera.Size;
            }
        }
    }
}
