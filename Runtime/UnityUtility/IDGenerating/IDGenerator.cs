namespace UU.IDGenerating
{
    public interface IDGenerator<T>
    {
        T LastID { get; }
        T GetNewId();
    }
}
