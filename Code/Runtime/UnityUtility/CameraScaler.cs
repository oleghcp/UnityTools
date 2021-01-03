using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityUtility
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class CameraScaler : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private Camera _camera;
        [SerializeField]
        private float _targetHeight = 3f;
        [SerializeField]
        private float _targetWidth = 4f;

        private float _targetRatio;
        private float _aspectRatio;

        private void Awake()
        {
            f_initialize();
        }

        private void Update()
        {
            float newRatio = f_getCurrentRatio();

            if (newRatio < _targetRatio)
            {
                if (_camera.orthographicSize != _targetHeight)
                    _camera.orthographicSize = _targetHeight;

                return;
            }

            if (_aspectRatio != newRatio)
            {
                _camera.orthographicSize = _targetWidth * newRatio;
                _aspectRatio = newRatio;
            }
        }

        private void OnValidate()
        {
            _camera = GetComponent<Camera>();
            f_initialize();
        }

        private void f_initialize()
        {
            _targetRatio = _targetHeight / _targetWidth;
            _aspectRatio = f_getCurrentRatio();
            if (_aspectRatio < _targetRatio)
                _camera.orthographicSize = _targetHeight;
            else
                _camera.orthographicSize = _targetWidth * _aspectRatio;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float f_getCurrentRatio()
        {
            return (float)Screen.height / Screen.width;
        }
    }
}
