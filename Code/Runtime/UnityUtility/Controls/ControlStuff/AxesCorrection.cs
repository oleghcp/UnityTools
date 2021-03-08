using System;

namespace UnityUtility.Controls.ControlStuff
{
    internal static class AxesCorrection
    {
        public static Func<GPAxisCode, float, float> GetAxisCorrectionFunc(GamepadType gamepad)
        {
            switch (gamepad)
            {
                case GamepadType.XBoxWin:
                    return PosRangeAxis;

                case GamepadType.DualShockWin:
                    return FullRangeAxis;

                case GamepadType.XBoxAndroid:
                case GamepadType.DualShockAndroid:
                    return FullRangeAxisInvertCross;

                case GamepadType.GoogleAndroid:
                    return PosRangeAxisInvertCross;

                default: throw new UnsupportedValueException(gamepad);
            }
        }

        // - Functions - //

        private static float PosRangeAxis(GPAxisCode axisCode, float value)
        {
            switch (axisCode)
            {
                case GPAxisCode.LStickY:
                case GPAxisCode.RStickY:
                    return -value;

                default:
                    return value;
            }
        }

        private static float PosRangeAxisInvertCross(GPAxisCode axisCode, float value)
        {
            switch (axisCode)
            {
                case GPAxisCode.Vertical:
                case GPAxisCode.LStickY:
                case GPAxisCode.RStickY:
                    return -value;

                default:
                    return value;
            }
        }

        private static float FullRangeAxis(GPAxisCode axisCode, float value)
        {
            switch (axisCode)
            {
                case GPAxisCode.LStickY:
                case GPAxisCode.RStickY:
                    return -value;

                case GPAxisCode.LeftTrgr:
                case GPAxisCode.RightTrgr:
                    return (value + 1f) * 0.5f;

                default:
                    return value;
            }
        }

        private static float FullRangeAxisInvertCross(GPAxisCode axisCode, float value)
        {
            switch (axisCode)
            {
                case GPAxisCode.Vertical:
                case GPAxisCode.LStickY:
                case GPAxisCode.RStickY:
                    return -value;

                case GPAxisCode.LeftTrgr:
                case GPAxisCode.RightTrgr:
                    return (value + 1f) * 0.5f;

                default:
                    return value;
            }
        }
    }
}
