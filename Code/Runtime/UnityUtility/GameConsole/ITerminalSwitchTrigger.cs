#if !UNITY_2019_2_OR_NEWER || INCLUDE_UNITY_UI

namespace UnityUtility.GameConsole
{
    public interface ITerminalSwitchTrigger
    {
        bool SwitchThisFrame { get; }
    }
}
#endif