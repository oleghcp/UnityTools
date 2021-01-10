using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityUtility
{
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class VerticalCameraScaler : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private Camera _camera;
        [SerializeField]
        private float _horizontalSize = 4f;

        private float _aspectRatio;

        private void Awake()
        {
            f_initialize();
        }

        private void Update()
        {
            float newRatio = f_getCurrentRatio();

            if (_aspectRatio != newRatio)
            {
                _camera.orthographicSize = _horizontalSize * newRatio;
                _aspectRatio = newRatio;
            }
        }

        private void OnValidate()
        {
            f_initialize();
        }

        private void f_initialize()
        {
            _camera = GetComponent<Camera>();
            _aspectRatio = f_getCurrentRatio();
            _camera.orthographicSize = _horizontalSize * _aspectRatio;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float f_getCurrentRatio()
        {
            return (float)Screen.height / Screen.width;
        }
    }
}
