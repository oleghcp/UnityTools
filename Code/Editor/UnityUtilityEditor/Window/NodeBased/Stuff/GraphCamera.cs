using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.MathExt;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class GraphCamera
    {
        private float _size = 1f;
        private GraphEditorWindow _window;

        public Vector2 Position
        {
            get => _window.GraphAssetEditor.CameraPosition;
            set => _window.GraphAssetEditor.CameraPosition = value;
        }

        public float Size
        {
            get => _size;
            set => _size = value.Clamp(0.1f, 10f);
        }

        public GraphCamera(GraphEditorWindow window)
        {
            _window = window;
        }

        public void Drag(Vector2 mouseDelta)
        {
            _window.GraphAssetEditor.CameraPosition -= mouseDelta * _size;
        }

        public Rect GetWorldRect()
        {
            Vector2 camSize = _window.position.size * _window.Camera.Size;
            return new Rect(_window.Camera.Position - camSize * 0.5f, camSize);
        }

        public Vector2 ScreenToWorld(Vector2 screenPoint)
        {
            return (screenPoint - GetWindowHalhSize()) * _size + Position;
        }

        public Vector2 WorldToScreen(Vector2 worldPoint)
        {
            return (worldPoint - Position) / _size + GetWindowHalhSize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector2 GetWindowHalhSize()
        {
            return _window.position.size * 0.5f;
        }
    }
}
