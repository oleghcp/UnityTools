#if INCLUDE_UNITY_UI

namespace UnityUtility.GameConsole
{
    public interface ITerminalSwitchTrigger
    {
        bool SwitchThisFrame { get; }
    }
}
#endif