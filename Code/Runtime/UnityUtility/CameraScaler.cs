using UnityEngine;
using UnityUtility;

namespace Project.Util
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public class CameraScaler : MonoBehaviour
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
            Initialize();
        }

        private void Update()
        {
            var newRatio = ScreenUtility.GetCurrentRatio();

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
            Initialize();
        }

        private void Initialize()
        {
            _targetRatio = _targetHeight / _targetWidth;
            _aspectRatio = ScreenUtility.GetCurrentRatio();
            if (_aspectRatio < _targetRatio)
                _camera.orthographicSize = _targetHeight;
            else
                _camera.orthographicSize = _targetWidth * _aspectRatio;

        }
    }
}
