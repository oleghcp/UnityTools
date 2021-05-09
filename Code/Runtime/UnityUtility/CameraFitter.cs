using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityUtility
{
    public enum AspectMode
    {
        FixedHeight,
        FixedWidth,
        EnvelopeAspect,
    }

    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class CameraFitter : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private Camera _camera;
        [SerializeField]
        private AspectMode _aspectMode;
        [SerializeField]
        private float _targetVertical;
        [SerializeField]
        private float _targetHorizontal;

        private float _currentAspect;

#if UNITY_EDITOR
        internal static string ModeFieldName => nameof(_aspectMode);
        internal static string VerticalFieldName => nameof(_targetVertical);
        internal static string HorizontalFieldName => nameof(_targetHorizontal);
#endif

        public Camera Camera => _camera;
        public AspectMode AspectMode
        {
            get => _aspectMode;
            set
            {
                if (_aspectMode == value)
                    return;

                _aspectMode = value;

                if (_camera.orthographic)
                    OrthoInit();
                else
                    PerspInit();
            }
        }

        private void Awake()
        {
            if (_camera == null)
                _camera = GetComponent<Camera>();

            if (_aspectMode == AspectMode.FixedHeight)
                return;

            _currentAspect = GetCurrentRatio();

            if (_camera.orthographic)
                OrthoInit();
            else
                PerspInit();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_camera.orthographic)
                OrthoInit();
            else
                PerspInit();
        }
#endif

        private void LateUpdate()
        {
            if (_aspectMode == AspectMode.FixedHeight)
                return;

            float newRatio = GetCurrentRatio();

            if (_currentAspect != newRatio)
            {
                _currentAspect = newRatio;

                if (_camera.orthographic)
                    OrthoInit();
                else
                    PerspInit();
            }
        }

        private void OrthoInit()
        {
            if (_aspectMode == AspectMode.FixedWidth)
            {
                _camera.orthographicSize = _targetVertical = _targetHorizontal * _currentAspect;
                return;
            }

            float targetRatio = _targetVertical / _targetHorizontal;

            if (targetRatio >= _currentAspect)
                _camera.orthographicSize = _targetVertical;
            else
                _camera.orthographicSize = _targetHorizontal * _currentAspect;
        }

        private void PerspInit()
        {
            if (_aspectMode == AspectMode.FixedWidth)
            {
                _camera.fieldOfView = _targetVertical = ScreenUtility.GetAspectAngle(_targetHorizontal, _currentAspect);
                return;
            }

            float vTan = ScreenUtility.GetHalfFovTan(_targetVertical);
            float hTan = ScreenUtility.GetHalfFovTan(_targetHorizontal);

            float targetRatio = vTan / hTan;

            if (targetRatio >= _currentAspect)
                _camera.fieldOfView = _targetVertical;
            else
                _camera.fieldOfView = ScreenUtility.GetAspectAngle(_targetHorizontal, _currentAspect);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetCurrentRatio()
        {
            return (float)Screen.height / Screen.width;
        }
    }
}
