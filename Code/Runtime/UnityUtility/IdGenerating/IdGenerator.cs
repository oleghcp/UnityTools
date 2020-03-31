namespace UnityUtility.IdGenerating
{
    public interface IdGenerator<T>
    {
        T LastID { get; }
        T GetNewId();
    }
}
