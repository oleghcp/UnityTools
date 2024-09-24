using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace OlegHcp
{
    public enum AspectMode
    {
        FixedHeight,
        FixedWidth,
        EnvelopeAspect,
    }

    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    [AddComponentMenu(nameof(OlegHcp) + "/Camera Fitter")]
    public sealed class CameraFitter : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private AspectMode _aspectMode;
        [SerializeField, FormerlySerializedAs("_targetVertical")]
        private float _targetVerticalSize;
        [SerializeField, FormerlySerializedAs("_targetHorizontal")]
        private float _targetHorizontalSize;
        [SerializeField, FormerlySerializedAs("_targetVertical")]
        private float _targetVerticalFov;
        [SerializeField, FormerlySerializedAs("_targetHorizontal")]
        private float _targetHorizontalFov;

        private bool _orthographic;
        private float _currentAspect;

#if UNITY_EDITOR
        internal static string CameraFieldName => nameof(_camera);
        internal static string ModeFieldName => nameof(_aspectMode);
        internal static string VerticalFieldSizeName => nameof(_targetVerticalSize);
        internal static string HorizontalFieldSizeName => nameof(_targetHorizontalSize);
        internal static string VerticalFieldFovName => nameof(_targetVerticalFov);
        internal static string HorizontalFieldFovName => nameof(_targetHorizontalFov);
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
                ApplyChanges(_currentAspect, _orthographic);
            }
        }

        public float TargetVerticalSize
        {
            get => _targetVerticalSize;
            set
            {
                if (_targetVerticalSize == value)
                    return;

                _targetVerticalSize = value;
                ApplyChanges(_currentAspect, _orthographic);
            }
        }

        public float TargetHorizontalSize
        {
            get => _targetHorizontalSize;
            set
            {
                if (_targetHorizontalSize == value)
                    return;

                _targetHorizontalSize = value;
                ApplyChanges(_currentAspect, _orthographic);
            }
        }

        public float TargetVerticalFov
        {
            get => _targetVerticalFov;
            set
            {
                if (_targetVerticalFov == value)
                    return;

                _targetVerticalFov = value;
                ApplyChanges(_currentAspect, _orthographic);
            }
        }

        public float TargetHorizontalFov
        {
            get => _targetHorizontalFov;
            set
            {
                if (_targetHorizontalFov == value)
                    return;

                _targetHorizontalFov = value;
                ApplyChanges(_currentAspect, _orthographic);
            }
        }

        private void Awake()
        {
            ParamsChanged();
            ApplyChanges(_currentAspect, _orthographic);
        }

        private void LateUpdate()
        {
            if (ParamsChanged())
                ApplyChanges(_currentAspect, _orthographic);
        }

        public float GetEnvelopeRatio()
        {
            if (_camera.orthographic)
                return _targetHorizontalSize / _targetVerticalSize;

            float vTan = ScreenUtility.GetHalfFovTan(_targetVerticalFov);
            float hTan = ScreenUtility.GetHalfFovTan(_targetHorizontalFov);
            return hTan / vTan;
        }

        internal void ApplyChanges(float currentAspect, bool orthographic)
        {
            switch (_aspectMode)
            {
                case AspectMode.FixedHeight:
                    if (orthographic)
                        _camera.orthographicSize = _targetVerticalSize;
                    else
                        _camera.fieldOfView = _targetVerticalFov;
                    break;

                case AspectMode.FixedWidth:
                    if (orthographic)
                        _camera.orthographicSize = _targetHorizontalSize * currentAspect;
                    else
                        _camera.fieldOfView = ScreenUtility.GetAspectAngle(_targetHorizontalFov, currentAspect);
                    break;

                case AspectMode.EnvelopeAspect:
                    if (orthographic)
                    {
                        if (currentAspect <= _targetVerticalSize / _targetHorizontalSize)
                            _camera.orthographicSize = _targetVerticalSize;
                        else
                            _camera.orthographicSize = _targetHorizontalSize * currentAspect;
                    }
                    else
                    {
                        float vTan = ScreenUtility.GetHalfFovTan(_targetVerticalFov);
                        float hTan = ScreenUtility.GetHalfFovTan(_targetHorizontalFov);

                        if (currentAspect <= vTan / hTan)
                            _camera.fieldOfView = _targetVerticalFov;
                        else
                            _camera.fieldOfView = ScreenUtility.GetAspectAngle(_targetHorizontalFov, currentAspect);
                    }
                    break;

                default:
                    throw new SwitchExpressionException(_aspectMode);
            }
        }

        private bool ParamsChanged()
        {
            bool ortho = _camera.orthographic;
            float newRatio = (float)Screen.height / Screen.width;

            if (_currentAspect != newRatio || _orthographic != ortho)
            {
                _currentAspect = newRatio;
                _orthographic = ortho;
                return true;
            }

            return false;
        }
    }
}
