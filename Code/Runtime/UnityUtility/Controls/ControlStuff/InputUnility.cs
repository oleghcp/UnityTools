using System;
using System.Text;
using UnityEngine;

namespace UnityUtility.Controls.ControlStuff
{
    public static class InputUnility
    {
        internal static KeyCode CreateKeyCode(KeyCode keyCodeRaw, int padNum, StringBuilder builder)
        {
            if (keyCodeRaw < KeyCode.JoystickButton0)
            {
                return keyCodeRaw;
            }
            else
            {
                Type type = typeof(KeyCode);
                string name = Enum.GetName(type, keyCodeRaw);
                int index = name.IndexOf('B');

                builder.Clear().Append(name.Substring(0, index));
                builder.Append(padNum);
                builder.Append(name.Substring(index));

                return (KeyCode)Enum.Parse(type, builder.ToString());
            }
        }

        internal static string AxisName(int axNum, int padNum, StringBuilder builder)
        {
            builder.Clear().Append((char)('@' + padNum));
            builder.Append(':');
            if (axNum < 10) { builder.Append('0'); }
            builder.Append(axNum);

            return builder.ToString();
        }

        internal static unsafe void GetRawKeyCodes(GamepadType type, KeyCode* array)
        {
            switch (type)
            {
                case GamepadType.XBoxWin:
                    array[0] = KeyCode.None;
                    array[1] = KeyCode.None;
                    array[2] = KeyCode.None;
                    array[3] = KeyCode.None;
                    array[4] = KeyCode.JoystickButton2;
                    array[5] = KeyCode.JoystickButton1;
                    array[6] = KeyCode.JoystickButton0;
                    array[7] = KeyCode.JoystickButton3;
                    array[8] = KeyCode.JoystickButton4;
                    array[9] = KeyCode.JoystickButton5;
                    array[10] = KeyCode.JoystickButton8;
                    array[11] = KeyCode.JoystickButton9;
                    array[12] = KeyCode.None;
                    array[13] = KeyCode.None;
                    array[14] = KeyCode.JoystickButton6;
                    array[15] = KeyCode.JoystickButton7;
                    break;

                case GamepadType.XBoxAndroid:
                    array[0] = KeyCode.None;
                    array[1] = KeyCode.None;
                    array[2] = KeyCode.None;
                    array[3] = KeyCode.None;
                    array[4] = KeyCode.JoystickButton2;
                    array[5] = KeyCode.JoystickButton1;
                    array[6] = KeyCode.JoystickButton0;
                    array[7] = KeyCode.JoystickButton3;
                    array[8] = KeyCode.LeftShift;
                    array[9] = KeyCode.RightShift;
                    array[10] = KeyCode.JoystickButton8;
                    array[11] = KeyCode.JoystickButton9;
                    array[12] = KeyCode.None;
                    array[13] = KeyCode.None;
                    array[14] = KeyCode.Pause;
                    array[15] = KeyCode.Return;
                    break;

                case GamepadType.DualShockWin:
                    array[0] = KeyCode.None;
                    array[1] = KeyCode.None;
                    array[2] = KeyCode.None;
                    array[3] = KeyCode.None;
                    array[4] = KeyCode.JoystickButton0;
                    array[5] = KeyCode.JoystickButton2;
                    array[6] = KeyCode.JoystickButton1;
                    array[7] = KeyCode.JoystickButton3;
                    array[8] = KeyCode.JoystickButton4;
                    array[9] = KeyCode.JoystickButton5;
                    array[10] = KeyCode.JoystickButton10;
                    array[11] = KeyCode.JoystickButton11;
                    array[12] = KeyCode.None;
                    array[13] = KeyCode.None;
                    array[14] = KeyCode.JoystickButton8;
                    array[15] = KeyCode.JoystickButton9;
                    break;

                case GamepadType.DualShockAndroid:
                    array[0] = KeyCode.None;
                    array[1] = KeyCode.None;
                    array[2] = KeyCode.None;
                    array[3] = KeyCode.None;
                    array[4] = KeyCode.JoystickButton0;
                    array[5] = KeyCode.JoystickButton13;
                    array[6] = KeyCode.JoystickButton1;
                    array[7] = KeyCode.JoystickButton2;
                    array[8] = KeyCode.JoystickButton3;
                    array[9] = KeyCode.JoystickButton14;
                    array[10] = KeyCode.Pause;
                    array[11] = KeyCode.Return;
                    array[12] = KeyCode.None;
                    array[13] = KeyCode.None;
                    array[14] = KeyCode.LeftAlt;
                    array[15] = KeyCode.RightAlt;
                    break;

                case GamepadType.GoogleAndroid:
                    array[0] = KeyCode.None;
                    array[1] = KeyCode.None;
                    array[2] = KeyCode.None;
                    array[3] = KeyCode.None;
                    array[4] = KeyCode.JoystickButton2;
                    array[5] = KeyCode.JoystickButton1;
                    array[6] = KeyCode.JoystickButton0;
                    array[7] = KeyCode.JoystickButton3;
                    array[8] = KeyCode.LeftShift;
                    array[9] = KeyCode.RightShift;
                    array[10] = KeyCode.JoystickButton8;
                    array[11] = KeyCode.JoystickButton9;
                    array[12] = KeyCode.None;
                    array[13] = KeyCode.None;
                    array[14] = KeyCode.Escape;
                    array[15] = KeyCode.Return;
                    break;

                default: throw new UnsupportedValueException(type);
            }
        }

        internal static unsafe void GetRawAxisCodes(GamepadType type, int* array)
        {
            switch (type)
            {
                case GamepadType.XBoxWin:
                    array[0] = 6;
                    array[1] = 7;
                    array[2] = 1;
                    array[3] = 2;
                    array[4] = 4;
                    array[5] = 5;
                    array[6] = 9;
                    array[7] = 10;
                    break;

                case GamepadType.XBoxAndroid:
                    array[0] = 5;
                    array[1] = 6;
                    array[2] = 1;
                    array[3] = 2;
                    array[4] = 3;
                    array[5] = 4;
                    array[6] = 14;
                    array[7] = 15;
                    break;

                case GamepadType.DualShockWin:
                    array[0] = 7;
                    array[1] = 8;
                    array[2] = 1;
                    array[3] = 2;
                    array[4] = 3;
                    array[5] = 6;
                    array[6] = 4;
                    array[7] = 5;
                    break;

                case GamepadType.DualShockAndroid:
                    array[0] = 5;
                    array[1] = 6;
                    array[2] = 1;
                    array[3] = 2;
                    array[4] = 14;
                    array[5] = 15;
                    array[6] = 3;
                    array[7] = 4;
                    break;

                case GamepadType.GoogleAndroid:
                    array[0] = 5;
                    array[1] = 6;
                    array[2] = 1;
                    array[3] = 2;
                    array[4] = 3;
                    array[5] = 4;
                    array[6] = 13;
                    array[7] = 12;
                    break;

                default: throw new UnsupportedValueException(type);
            }
        }

        public static KMKeyCode GetPressedKey()
        {
            if (Input.anyKeyDown)
            {
                for (int i = 0; i < 500; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i))
                    { return (KMKeyCode)i; }
                }
            }

            return KMKeyCode.None;
        }

        public static unsafe GPKeyCode GetPressedKey(GamepadType type)
        {
            if (Input.anyKeyDown)
            {
                KeyCode* codes = stackalloc KeyCode[InputEnum.GPKeyCodeCount];
                GetRawKeyCodes(type, codes);

                for (int i = 0; i < InputEnum.GPKeyCodeCount; i++)
                {
                    if (Input.GetKeyDown(codes[i]))
                    { return (GPKeyCode)i; }
                }
            }

            return GPKeyCode.None;
        }
    }
}
