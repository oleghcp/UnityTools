using System;

namespace UU.Controls.ControlStuff
{
    internal static class AxesCorrection
    {
        internal static Func<GPAxisCode, float, float> GetAxisCorrectionFunc(GamepadType gamepad)
        {
            switch (gamepad)
            {
                case GamepadType.XBoxWin:
                    return f_posRangeAxis;

                case GamepadType.DualShockWin:
                    return f_fullRangeAxis;

                case GamepadType.XBoxAndroid:
                case GamepadType.DualShockAndroid:
                    return f_fullRangeAxisInvertCross;

                case GamepadType.GoogleAndroid:
                    return f_posRangeAxisInvertCross;

                default: throw new UnsupportedValueException(gamepad);
            }
        }

        // - Functions - //

        private static float f_posRangeAxis(GPAxisCode axisCode, float value)
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

        private static float f_posRangeAxisInvertCross(GPAxisCode axisCode, float value)
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

        private static float f_fullRangeAxis(GPAxisCode axisCode, float value)
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

        private static float f_fullRangeAxisInvertCross(GPAxisCode axisCode, float value)
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
