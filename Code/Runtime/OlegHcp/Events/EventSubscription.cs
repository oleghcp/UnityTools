using System;

namespace OlegHcp.Events
{
    internal struct EventSubscription : IEquatable<EventSubscription>, IComparable<EventSubscription>
    {
        private object _handler;
        private int _priority;

        public EventSubscription(object handler, int priority = int.MaxValue)
        {
            _handler = handler;
            _priority = priority;
        }

        public void Invoke<TSignal>(TSignal signal) where TSignal : ISignal
        {
            if (_handler is Action<TSignal> func)
            {
                func.Invoke(signal);
                return;
            }

            ((IEventListener<TSignal>)_handler).HandleEvent(signal);
        }

        public bool Equals(EventSubscription other)
        {
            return _handler == other._handler;
        }

        public override bool Equals(object other)
        {
            return other is EventSubscription s && Equals(s);
        }

        public override int GetHashCode()
        {
            return _handler.GetHashCode();
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
