namespace UnityUtility.IdGenerating
{
    public interface IIdGenerator<T>
    {
        T LastID { get; }
        T GetNewId();
    }
}
