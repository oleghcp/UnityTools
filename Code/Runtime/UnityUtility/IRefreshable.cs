namespace OlegHcp
{
    public interface IRefreshable
    {
        void Refresh();
    }

    public interface IUpdateable
    {
        void Refresh(float deltaTime);
    }
}
