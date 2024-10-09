namespace OlegHcp.Mathematics
{
    public interface ICurve<TVector>
    {
        int Count { get; }
        TVector this[int index] { get; set; }
        TVector Evaluate(float ratio);
    }
}
