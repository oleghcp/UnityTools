namespace UnityUtility.Controls
{
    public interface IInputObtainer : IRefreshable
    {
        ButtonState GetKeyState(int keyAction);
        float GetAxisValue(int axisAction);
        void Reset();
    }
}
