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
            if (orthographic)
                orthoInit();
            else
                perspInit();

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
                        _camera.orthographicSize = _targetVerticalSize = _targetHorizontalSize * currentAspect;
                    else
                        _camera.fieldOfView = _targetVerticalFov = ScreenUtility.GetAspectAngle(_targetHorizontalFov, currentAspect);
                    break;

                case AspectMode.EnvelopeAspect:
                    if (orthographic)
                        orthoInit();
                    else
                        perspInit();
                    break;

                default:
                    throw new UnsupportedValueException(_aspectMode);
            }

            void orthoInit()
            {
                float targetRatio = _targetVerticalSize / _targetHorizontalSize;

                if (targetRatio >= currentAspect)
                    _camera.orthographicSize = _targetVerticalSize;
                else
                    _camera.orthographicSize = _targetHorizontalSize * currentAspect;
            }

            void perspInit()
            {
                float vTan = ScreenUtility.GetHalfFovTan(_targetVerticalFov);
                float hTan = ScreenUtility.GetHalfFovTan(_targetHorizontalFov);

                float targetRatio = vTan / hTan;

                if (targetRatio >= currentAspect)
                    _camera.fieldOfView = _targetVerticalFov;
                else
                    _camera.fieldOfView = ScreenUtility.GetAspectAngle(_targetHorizontalFov, currentAspect);
            }
        }

        private bool ParamsChanged()
        {
            bool ortho = _camera.orthographic;
            float newRatio = (float)Screen.height / Screen.width;

            if (_currentAspect == newRatio && _orthographic == ortho)
                return false;

            _currentAspect = newRatio;
            _orthographic = ortho;
            return true;
        }
    }
}
