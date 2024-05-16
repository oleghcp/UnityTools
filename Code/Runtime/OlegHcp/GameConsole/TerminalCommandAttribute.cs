#if INCLUDE_UNITY_UI
using System;

namespace OlegHcp.GameConsole
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TerminalCommandAttribute : Attribute
    {
        internal string CommandName { get; }

        public TerminalCommandAttribute() { }

        public TerminalCommandAttribute(string commandName)
        {
            CommandName = commandName;
        }
    }
}
#endif
