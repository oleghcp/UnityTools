#if !UNITY_2019_3_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER
using System;
using System.Text;
using UnityEngine;

namespace UnityUtility.Controls.ControlStuff
{
    internal class GamepadInputConverter
    {
        public readonly KeyCode[] KeyCodes;
        public readonly string[] AxisNames;

        public GamepadInputConverter(GamepadType type, int padNum)
        {
            padNum++;
            StringBuilder builder = new StringBuilder();

            Span<KeyCode> rawKeyCodes = stackalloc KeyCode[InputEnumUtility.GPKeyCodeCount];
            InputUnility.GetRawKeyCodes(type, rawKeyCodes);
            KeyCodes = new KeyCode[InputEnumUtility.GPKeyCodeCount];
            for (int i = 0; i < InputEnumUtility.GPKeyCodeCount; i++)
                KeyCodes[i] = InputUnility.CreateKeyCode(rawKeyCodes[i], padNum, builder);

            Span<int> axisCodes = stackalloc int[InputEnumUtility.GPAxisCodeCount];
            InputUnility.GetRawAxisCodes(type, axisCodes);
            AxisNames = new string[InputEnumUtility.GPAxisCodeCount];
            for (int i = 0; i < InputEnumUtility.GPAxisCodeCount; i++)
                AxisNames[i] = InputUnility.AxisName(axisCodes[i], padNum, builder);
        }
    }
}
#endif
