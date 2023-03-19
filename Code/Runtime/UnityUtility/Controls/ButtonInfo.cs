using System;
using UnityUtility.Tools;

#if !UNITY_2019_3_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER
namespace UnityUtility.Controls
{
    public struct ButtonInfo : IEquatable<ButtonInfo>
    {
        internal int Function;
        internal int KeyCode;

        public override bool Equals(object obj)
        {
            return obj is ButtonInfo buttonInfo && Equals(buttonInfo);
        }

        public bool Equals(ButtonInfo other)
        {
            return Function == other.Function && KeyCode == other.KeyCode;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(Function.GetHashCode(), KeyCode.GetHashCode());
        }

        public static bool operator ==(ButtonInfo a, ButtonInfo b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ButtonInfo a, ButtonInfo b)
        {
            return !a.Equals(b);
        }
    }
}
#endif
