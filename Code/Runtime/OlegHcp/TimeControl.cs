using System.Runtime.CompilerServices;
using OlegHcp.Mathematics;
using UnityEngine;

namespace OlegHcp
{
    public static class TimeControl
    {
        private static float _scale = 1f;
        private static bool _paused;

        public static bool Paused
        {
            get => _paused;
            set
            {
                if (_paused == value)
                    return;

                _paused = value;
                Time.timeScale = value ? 0f : _scale.ClampMin(0f);
            }
        }

        public static float Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                ApplyScale();
            }
        }

        public static void ResetScale()
        {
            _scale = 1f;
            ApplyScale();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyScale()
        {
            if (!_paused)
                Time.timeScale = _scale.ClampMin(0f);
        }
    }
}
