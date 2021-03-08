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

            Span<KeyCode> rawKeyCodes = stackalloc KeyCode[InputEnum.GPKeyCodeCount];
            InputUnility.GetRawKeyCodes(type, rawKeyCodes);
            KeyCodes = new KeyCode[InputEnum.GPKeyCodeCount];
            for (int i = 0; i < InputEnum.GPKeyCodeCount; i++)
                KeyCodes[i] = InputUnility.CreateKeyCode(rawKeyCodes[i], padNum, builder);

            Span<int> axisCodes = stackalloc int[InputEnum.GPAxisCodeCount];
            InputUnility.GetRawAxisCodes(type, axisCodes);
            AxisNames = new string[InputEnum.GPAxisCodeCount];
            for (int i = 0; i < InputEnum.GPAxisCodeCount; i++)
                AxisNames[i] = InputUnility.AxisName(axisCodes[i], padNum, builder);
        }
    }
}
