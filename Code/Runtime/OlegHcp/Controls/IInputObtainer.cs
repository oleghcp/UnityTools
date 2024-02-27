#if !UNITY_2019_3_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER
namespace OlegHcp.Controls
{
    public interface IInputObtainer : IRefreshable
    {
        ButtonState GetKeyState(int keyAction);
        float GetAxisValue(int axisAction);
        void Reset();
    }
}
#endif
