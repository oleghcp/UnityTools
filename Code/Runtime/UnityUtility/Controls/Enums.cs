using UnityUtility.Controls.ControlStuff;
using System;
using UnityEngine;

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

    public enum ButtonState : byte { None = 0, Down, Stay, Up }

    //DO NOT CHANGE THE ORDER!
    public enum GPKeyCode : sbyte
    {
        None = -1,
        LeftArrow,
        RightArrow,
        DownArrow,
        UpArrow,
        ActionL,
        ActionR,
        ActionB,
        ActionT,
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

    public enum KMAxisCode : sbyte
    {
        None = -1,
        MouseX,
        MouseY,
        Wheel,
        Horizontal,
        Vertical
    }

    internal static class InputEnum
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
            }

            throw new InvalidOperationException($"Unsupported type: {type.GetName()}.");
        }

        public static int GetAxisDefVal(int inputType)
        {
            InputType type = (InputType)inputType;

            switch (type)
            {
                case InputType.KeyMouse: return (int)KMAxisCode.None;
                case InputType.Gamepad: return (int)GPAxisCode.None;
            }

            throw new InvalidOperationException($"Unsupported type: {type.GetName()}.");
        }
    }
}
