#if INCLUDE_UNITY_UI

namespace OlegHcp.GameConsole
{
    public interface ITerminalSwitchTrigger
    {
        bool SwitchThisFrame { get; }
    }
}
#endif