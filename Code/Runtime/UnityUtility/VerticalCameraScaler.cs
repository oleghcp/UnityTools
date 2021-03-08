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
            Initialize();
        }

        private void Update()
        {
            float newRatio = GetCurrentRatio();

            if (_aspectRatio != newRatio)
            {
                _camera.orthographicSize = _horizontalSize * newRatio;
                _aspectRatio = newRatio;
            }
        }

        private void OnValidate()
        {
            Initialize();
        }

        private void Initialize()
        {
            _camera = GetComponent<Camera>();
            _aspectRatio = GetCurrentRatio();
            _camera.orthographicSize = _horizontalSize * _aspectRatio;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetCurrentRatio()
        {
            return (float)Screen.height / Screen.width;
        }
    }
}
