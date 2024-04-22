namespace OlegHcp.Events
{
    public abstract class BusEvent
    {
        public abstract bool HasOwner { get; }
        public abstract void Invoke<TSignal>(TSignal signal) where TSignal : ISignal;
        public abstract void UnregisterOwner();
    }
}
