#if !UNITY_2019_2_OR_NEWER || INCLUDE_UNITY_UI
using System;

namespace UnityUtility.GameConsole
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TerminalCommandAttribute : Attribute
    {

    }
}
#endif
