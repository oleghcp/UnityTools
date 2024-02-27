#if !UNITY_2019_3_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER
using System;
using System.Text;
using UnityEngine;

namespace OlegHcp.Controls.ControlStuff
{
    internal class GamepadInputConverter
    {
        public readonly KeyCode[] KeyCodes;
        public readonly string[] AxisNames;

        public GamepadInputConverter(GamepadType type, int padNum)
        {
            padNum++;
            StringBuilder builder = new StringBuilder();

            Span<KeyCode> rawKeyCodes = stackalloc KeyCode[InputEnumUtility.GP_KEY_CODE_COUNT];
            InputUnility.GetRawKeyCodes(type, rawKeyCodes);
            KeyCodes = new KeyCode[InputEnumUtility.GP_KEY_CODE_COUNT];
            for (int i = 0; i < InputEnumUtility.GP_KEY_CODE_COUNT; i++)
                KeyCodes[i] = InputUnility.CreateKeyCode(rawKeyCodes[i], padNum, builder);

            Span<int> axisCodes = stackalloc int[InputEnumUtility.GP_AXIS_CODE_COUNT];
            InputUnility.GetRawAxisCodes(type, axisCodes);
            AxisNames = new string[InputEnumUtility.GP_AXIS_CODE_COUNT];
            for (int i = 0; i < InputEnumUtility.GP_AXIS_CODE_COUNT; i++)
                AxisNames[i] = InputUnility.AxisName(axisCodes[i], padNum, builder);
        }
    }
}
#endif
