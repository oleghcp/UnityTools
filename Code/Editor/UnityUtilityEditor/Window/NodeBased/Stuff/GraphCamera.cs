using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class GraphCamera
    {
        private GraphEditorWindow _window;
        private float _sizeFactor = 1f;
        private Rect _rect;
        private Vector2 _position;
        private bool _isDragging;
        private int _onGuiCounter;

        public float Size => _sizeFactor;
        public bool IsDragging => _isDragging;

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public Rect WorldRect
        {
            get
            {
                if (_onGuiCounter != _window.OnGuiCounter)
                {
                    Vector2 size = _window.position.size * _sizeFactor;
                    _rect.size = size;
                    _rect.position = _position - size * 0.5f;
                    _onGuiCounter = _window.OnGuiCounter;
                }

                return _rect;
            }
        }

        public GraphCamera(GraphEditorWindow window, Vector2 position)
        {
            _window = window;
            _position = position;
        }

        public Vector2 ScreenToWorld(Vector2 screenPoint)
        {
            return (screenPoint - GetWindowHalfSize()) * _sizeFactor + _position;
        }

        public Vector2 WorldToScreen(Vector2 worldPoint)
        {
            return (worldPoint - _position) / _sizeFactor + GetWindowHalfSize();
        }

        public void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 2)
                    {
                        _isDragging = true;
                        GUI.changed = true;
                    }
                    break;

                case EventType.MouseUp:
                    if (e.button == 2)
                    {
                        _isDragging = false;
                        GUI.changed = true;
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 2)
                    {
                        _position -= e.delta * _sizeFactor;
                        GUI.changed = true;
                    }
                    break;

                case EventType.ScrollWheel:
                    _sizeFactor = e.delta.y > 0f ? 2f : 1f;
                    GUI.changed = true;
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector2 GetWindowHalfSize()
        {
            return _window.position.size * 0.5f;
        }
    }
}
