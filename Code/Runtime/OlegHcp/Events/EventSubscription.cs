using System;

namespace OlegHcp.Events
{
    internal struct EventSubscription : IEquatable<EventSubscription>, IComparable<EventSubscription>
    {
        private object _handler;
        private int _priority;
        private int _hashCode;

        public int Priority => Priority;

        public EventSubscription(object handler, int priority)
        {
            _handler = handler;
            _priority = priority;
            _hashCode = handler.GetHashCode();
        }

        public void Invoke<TSignal>(TSignal signal) where TSignal : ISignal
        {
            if (_handler is Action<TSignal> func)
                func.Invoke(signal);
            else
                ((IEventListener<TSignal>)_handler).HandleEvent(signal);
        }

        public bool Equals(EventSubscription other)
        {
            return _hashCode == other._hashCode;
        }

        public override bool Equals(object other)
        {
            return other is EventSubscription s && Equals(s);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public int CompareTo(EventSubscription other)
        {
            return _priority.CompareTo(other._priority);
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
