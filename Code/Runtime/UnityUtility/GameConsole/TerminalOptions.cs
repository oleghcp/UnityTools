#if !UNITY_2019_2_OR_NEWER || INCLUDE_UNITY_UI
using UnityUtility.MathExt;

namespace UnityUtility.GameConsole
{
    public class TerminalOptions
    {
        private float _targetHeight = 0.75f;
        private int _linesLimit = 100;

        public bool AddSpaceAfterName = true;
        public bool ShowDebugLogs = true;

        /// <summary>Value of termial height relative screen (from 0f to 1f).</summary>
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
    }
}
#endif
