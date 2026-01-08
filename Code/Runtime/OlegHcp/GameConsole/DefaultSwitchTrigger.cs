#if INCLUDE_UNITY_UI
using OlegHcp.Tools;

namespace OlegHcp.GameConsole
{
    internal class DefaultSwitchTrigger : ITerminalSwitchTrigger
    {
        public bool SwitchThisFrame => ButtonUtility.BackQuoteDown;
    }
}
#endif
