using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class PointViewer
    {
        private const float RADIUS = 6f;

        private SerializedProperty _property;
        private TransitionViewer _transitionViewer;
        private GraphEditorWindow _window;
        private bool _isDragged;
        private Vector2 _dragedPosition;

        public Vector2 Position
        {
            get => _property.vector2Value;
            set => _property.vector2Value = value;
        }

        public PointViewer(SerializedProperty pointProperty, TransitionViewer transitionViewer, GraphEditorWindow window)
        {
            _property = pointProperty;
            _transitionViewer = transitionViewer;
            _window = window;
        }

        public void Draw(in Color color)
        {
            Handles.color = color;
            Handles.DrawSolidDisc(_window.Camera.WorldToScreen(Position), Vector3.back, RADIUS);
            Handles.color = Colours.White;
        }

        public bool ProcessEvents(Event e)
        {
            bool needLock = false;

            switch (e.type)
            {
                case EventType.MouseDown:
                    Rect pointRect = new Rect(_window.Camera.WorldToScreen(Position) - Vector2.one * RADIUS, Vector2.one * (RADIUS * 2f));

                    if (e.button == 0)
                    {
                        if (pointRect.Contains(e.mousePosition))
                        {
                            if (e.control)
                            {
                                _dragedPosition = Position;
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
                        _property.serializedObject.ApplyModifiedProperties();
                        GUI.changed = true;
                        needLock = true;
                    }
                    break;
            }

            return needLock;
        }

        private void Drag(Vector2 mouseDelta)
        {
            if (GraphEditorWindow.GridSnapping)
            {
                _dragedPosition += mouseDelta * _window.Camera.Size;
                Position = new Vector2(_dragedPosition.x.Round(GraphGrid.SMALL_STEP), _dragedPosition.y.Round(GraphGrid.SMALL_STEP));
            }
            else
            {
                Position += mouseDelta * _window.Camera.Size;
            }
        }
    }
}
#endif
