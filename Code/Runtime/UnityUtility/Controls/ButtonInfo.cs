using System;
using UnityUtilityTools;

#if !UNITY_2019_3_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER
namespace UnityUtility.Controls
{
    public struct ButtonInfo : IEquatable<ButtonInfo>
    {
        internal int Function;
        internal int KeyCode;

        public override bool Equals(object obj)
        {
            return obj is ButtonInfo buttonInfo && this == buttonInfo;
        }

        public bool Equals(ButtonInfo other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(Function.GetHashCode(), KeyCode.GetHashCode());
        }

        public static bool operator ==(ButtonInfo a, ButtonInfo b)
        {
            return a.Function == b.Function && a.KeyCode == b.KeyCode;
        }

        public static bool operator !=(ButtonInfo a, ButtonInfo b)
        {
            return !(a == b);
        }
    }
}
#endif
