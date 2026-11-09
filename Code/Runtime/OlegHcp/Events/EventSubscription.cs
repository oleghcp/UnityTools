using System;
using System.Runtime.CompilerServices;

namespace OlegHcp.Events
{
    internal struct EventSubscription : IEquatable<EventSubscription>, IComparable<EventSubscription>
    {
        private object _handler;
        private int _priority;
        private int _hashCode;

        public int Priority
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _priority;
        }

        public int HashCode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _hashCode;
        }

        public EventSubscription(object handler, int priority)
        {
            _handler = handler;
            _priority = priority;
            _hashCode = handler.GetHashCode();
        }

        public EventSubscription(int hash)
        {
            _hashCode = hash;
            _handler = default;
            _priority = default;
        }

        public void Invoke<TSignal>(TSignal signal) where TSignal : ISignal
        {
            if (_handler is IEventListener<TSignal> handler)
            {
                handler.HandleEvent(signal);
                return;
            }

            ((Action<TSignal>)_handler).Invoke(signal);

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
