using System;

namespace OlegHcp.Events
{
    internal struct EventSubscription : IEquatable<EventSubscription>
    {
        private Delegate _callback;

        public int Priority { get; }
        public Delegate Callback => _callback;

        public EventSubscription(Delegate callback, int priority = int.MaxValue)
        {
            _callback = callback;
            Priority = priority;
        }

        public void Invoke<T>(T signal) where T : ISignal
        {
            ((Action<T>)_callback).Invoke(signal);
        }

        public bool Equals(EventSubscription other)
        {
            return _callback == other._callback;
        }

        public override bool Equals(object other)
        {
            return other is EventSubscription s && Equals(s);
        }

        public override int GetHashCode()
        {
            return _callback.GetHashCode();
        }

        public static bool operator ==(EventSubscription a, EventSubscription b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(EventSubscription a, EventSubscription b)
        {
            return !a.Equals(b);
        }
    }
}
