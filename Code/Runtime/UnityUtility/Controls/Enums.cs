#if !UNITY_2019_3_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine;
using UnityUtility.Controls.ControlStuff;

namespace UnityUtility.Controls
{
    public enum GamepadType : byte
    {
        XBoxWin,
        XBoxAndroid,
        DualShockWin,
        DualShockAndroid,
        GoogleAndroid
    }

    public enum ButtonState : byte { None = 0, Down, Hold, Up }

    //DO NOT CHANGE THE ORDER!
    public enum GPKeyCode : sbyte
    {
        None = -1,
        ArrowLeft,
        ArrowRight,
        ArrowDown,
        ArrowUp,
        ActionWest,
        ActionEast,
        ActionSouth,
        ActionNorth,
        LeftBumper,
        RightBumper,
        LeftStick,
        RightStick,
        LeftTrgr,
        RightTrgr,
        S1,
        S2
    }

    //DO NOT CHANGE THE ORDER!
    public enum GPAxisCode : sbyte
    {
        None = -1,
        Horizontal,
        Vertical,
        LStickX,
        LStickY,
        RStickX,
        RStickY,
        LeftTrgr,
        RightTrgr
    }

    //DO NOT CHANGE THE ORDER!
    public enum KMAxisCode : sbyte
    {
        None = -1,
        MouseX,
        MouseY,
        Wheel,
        Horizontal,
        Vertical
    }

    internal static class InputEnumUtility
    {
        public const int GPKeyCodeCount = 16;
        public const int GPAxisCodeCount = 8;
        public const int KMAxisCodeCount = 5;

        public static int GetKeyDefVal(int inputType)
        {
            InputType type = (InputType)inputType;

            switch (type)
            {
                case InputType.KeyMouse: return (int)KeyCode.None;
                case InputType.Gamepad: return (int)GPKeyCode.None;
                default: throw new UnsupportedValueException(type);
            }
        }

        public static int GetAxisDefVal(int inputType)
        {
            InputType type = (InputType)inputType;

            switch ((InputType)inputType)
            {
                case InputType.KeyMouse: return (int)KMAxisCode.None;
                case InputType.Gamepad: return (int)GPAxisCode.None;
                default: throw new UnsupportedValueException(type);
            }
        }
    }
}
#endif
