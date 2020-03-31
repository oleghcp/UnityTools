using UnityEngine;
using System.Text;

namespace UnityUtility.Controls.ControlStuff
{
    internal class GamepadInputConverter
    {
        internal readonly KeyCode[] KeyCodes;
        internal readonly string[] AxisNames;

        internal GamepadInputConverter(GamepadType type, int padNum)
        {
            padNum++;
            StringBuilder builder = new StringBuilder();

            unsafe
            {
                KeyCode* rawKeyCodes = stackalloc KeyCode[InputEnum.GPKeyCodeCount];
                InputUnility.GetRawKeyCodes(type, rawKeyCodes);
                KeyCodes = new KeyCode[InputEnum.GPKeyCodeCount];
                for (int i = 0; i < InputEnum.GPKeyCodeCount; i++)
                    KeyCodes[i] = InputUnility.CreateKeyCode(rawKeyCodes[i], padNum, builder);
            }

            unsafe
            {
                int* axisCodes = stackalloc int[InputEnum.GPAxisCodeCount];
                InputUnility.GetRawAxisCodes(type, axisCodes);
                AxisNames = new string[InputEnum.GPAxisCodeCount];
                for (int i = 0; i < InputEnum.GPAxisCodeCount; i++)
                    AxisNames[i] = InputUnility.AxisName(axisCodes[i], padNum, builder);
            }
        }
    }
}
