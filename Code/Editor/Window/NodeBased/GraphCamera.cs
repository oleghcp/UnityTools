using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Window.NodeBased
{
    internal class GraphCamera
    {
        private float _sizeFactor = 1f;
        private Rect _rect;
        private Vector2 _position;
        private bool _isDragging;
        private int _worldRectVersion;
        private Vector2 _mapSize;

        public float Size => _sizeFactor;

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public Rect WorldRect
        {
            get
            {
                if (_worldRectVersion != GraphEditorWindow.OnGuiCounter)
                {
                    Vector2 size = _mapSize * _sizeFactor;
                    _rect.size = size;
                    _rect.position = _position - size * 0.5f;
                    _worldRectVersion = GraphEditorWindow.OnGuiCounter;
                }

                return _rect;
            }
        }

        public GraphCamera(Vector2 position)
        {
            _position = position;
        }

        public Vector2 ScreenToWorld(Vector2 screenPoint)
        {
            return (screenPoint - _mapSize * 0.5f) * _sizeFactor + _position;
        }

        public Vector2 WorldToScreen(Vector2 worldPoint)
        {
            return (worldPoint - _position) / _sizeFactor + _mapSize * 0.5f;
        }

        public void ProcessEvents(Event e, in Rect mapRect)
        {
            _mapSize = mapRect.size;

            if (!mapRect.Contains(e.mousePosition))
                return;

            bool changed = false;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 2)
                    {
                        _isDragging = true;
                        changed = true;
                    }
                    break;

                case EventType.MouseUp:
                    if (e.button == 2)
                    {
                        _isDragging = false;
                        changed = true;
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 2)
                    {
                        _isDragging = true;
                        _position -= e.delta * _sizeFactor;
                        changed = true;
                    }
                    break;

                case EventType.ScrollWheel:
                    _sizeFactor = e.delta.y > 0f ? 2f : 1f;
                    changed = true;
                    break;
            }

            if (changed)
            {
                e.Use();
                GUI.changed = true;
            }

            if (_isDragging)
                EditorGUIUtility.AddCursorRect(mapRect, MouseCursor.Pan);
        }
    }
}
