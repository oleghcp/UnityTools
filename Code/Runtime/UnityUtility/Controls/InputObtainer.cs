namespace UnityUtility.Controls
{
    public interface InputObtainer : Refreshable
    {
        ButtonState GetKeyState(int keyAction);
        float GetAxisValue(int axisAction);
        void Reset();
    }
}
