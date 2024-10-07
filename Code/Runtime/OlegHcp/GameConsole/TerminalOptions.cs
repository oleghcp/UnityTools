#if INCLUDE_UNITY_UI
using OlegHcp.Mathematics;

namespace OlegHcp.GameConsole
{
    public class TerminalOptions
    {
        private float _targetHeight = 0.75f;
        private int _linesLimit = 100;
        private float _moveSpeed = 10f;

        public bool AddSpaceAfterName = true;
        public bool ShowDebugLogs = true;

        /// <summary>Value of terminal height relative screen (from 0f to 1f).</summary>
        public float TargetHeight
        {
            get => _targetHeight;
            set => _targetHeight = value.Clamp01();
        }

        public int LinesLimit
        {
            get => _linesLimit;
            set => _linesLimit = value.ClampMin(0);
        }

        public float MoveSpeed
        {
            get => _moveSpeed;
            set => _moveSpeed = value.ClampMin(0f);
        }
    }
}
#endif
