using UnityEngine;

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
        [SerializeField]
        private float _targetVertical = 30f;
        [SerializeField]
        private float _targetHorizontal = 40f;

        private float _currentAspect;

#if UNITY_EDITOR
        internal static string CameraFieldName => nameof(_camera);
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

                ApplyChanges(_currentAspect);
            }
        }

        private void Awake()
        {
            RatioChanged();
            ApplyChanges(_currentAspect);
        }

        private void LateUpdate()
        {
            if (RatioChanged())
                ApplyChanges(_currentAspect);
        }

        public float GetEnvelopeRatio()
        {
            if (_camera.orthographic)
                return _targetHorizontal / _targetVertical;

            float vTan = ScreenUtility.GetHalfFovTan(_targetVertical);
            float hTan = ScreenUtility.GetHalfFovTan(_targetHorizontal);
            return hTan / vTan;
        }

        internal void ApplyChanges(float currentAspect)
        {
            if (_camera.orthographic)
                orthoInit();
            else
                perspInit();

            bool ortho = _camera.orthographic;

            switch (_aspectMode)
            {
                case AspectMode.FixedHeight:
                    if (ortho)
                        _camera.orthographicSize = _targetVertical;
                    else
                        _camera.fieldOfView = _targetVertical;
                    break;

                case AspectMode.FixedWidth:
                    if (ortho)
                        _camera.orthographicSize = _targetVertical = _targetHorizontal * currentAspect;
                    else
                        _camera.fieldOfView = _targetVertical = ScreenUtility.GetAspectAngle(_targetHorizontal, currentAspect);
                    break;

                case AspectMode.EnvelopeAspect:
                    if (ortho)
                        orthoInit();
                    else
                        perspInit();
                    break;

                default:
                    throw new UnsupportedValueException(_aspectMode);
            }

            void orthoInit()
            {
                float targetRatio = _targetVertical / _targetHorizontal;

                if (targetRatio >= currentAspect)
                    _camera.orthographicSize = _targetVertical;
                else
                    _camera.orthographicSize = _targetHorizontal * currentAspect;
            }

            void perspInit()
            {
                float vTan = ScreenUtility.GetHalfFovTan(_targetVertical);
                float hTan = ScreenUtility.GetHalfFovTan(_targetHorizontal);

                float targetRatio = vTan / hTan;

                if (targetRatio >= currentAspect)
                    _camera.fieldOfView = _targetVertical;
                else
                    _camera.fieldOfView = ScreenUtility.GetAspectAngle(_targetHorizontal, currentAspect);
            }
        }

        private bool RatioChanged()
        {
            float newRatio = (float)Screen.height / Screen.width;

            if (_currentAspect == newRatio)
                return false;

            _currentAspect = newRatio;
            return true;
        }
    }
}
