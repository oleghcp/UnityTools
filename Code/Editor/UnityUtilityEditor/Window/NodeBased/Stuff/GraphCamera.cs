using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.MathExt;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class GraphCamera
    {
        private GraphEditorWindow _window;
        private float _sizeFactor = 1f;
        private Rect _rect;
        private Vector2 _position;

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public float Size
        {
            get => _sizeFactor;
            set => _sizeFactor = value.Clamp(0.1f, 10f);
        }

        public GraphCamera(GraphEditorWindow window, Vector2 position)
        {
            _window = window;
            _position = position;
        }

        public void Drag(Vector2 mouseDelta)
        {
            _position -= mouseDelta * _sizeFactor;
        }

        public Rect GetWorldRect()
        {
            Vector2 size = _window.position.size * _sizeFactor;
            _rect.size = size;
            _rect.position = _position - size * 0.5f;
            return _rect;
        }

        public Vector2 ScreenToWorld(Vector2 screenPoint)
        {
            return (screenPoint - GetWindowHalfSize()) * _sizeFactor + _position;
        }

        public Vector2 WorldToScreen(Vector2 worldPoint)
        {
            return (worldPoint - _position) / _sizeFactor + GetWindowHalfSize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector2 GetWindowHalfSize()
        {
            return _window.position.size * 0.5f;
        }
    }
}
