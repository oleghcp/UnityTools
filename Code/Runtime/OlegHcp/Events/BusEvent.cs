using System;
using System.Collections.Generic;

namespace OlegHcp.Events
{
    public abstract class BusEvent
    {
        public abstract void Invoke<TSignal>(TSignal signal) where TSignal : ISignal;
        public abstract void Invoke<TSignal>() where TSignal : ISignal;
        public abstract void UnregisterOwner();
    }

    internal class InternalEvent : BusEvent
    {
        private List<EventSubscription> _subscriptions = new List<EventSubscription>();
        private object _owner;
        private bool _changed;

        public Type SignalType { get; }
        public object Owner => _owner;

        public InternalEvent(Type signalType)
        {
            SignalType = signalType;
        }

        public bool TrySetOwner(object owner)
        {
            if (_owner != null)
                return false;

            _owner = owner;
            return true;
        }

        public override void UnregisterOwner()
        {
            _owner = null;
        }

        public void Add(object handler, int priority)
        {
            if (priority != int.MaxValue)
                _changed = true;

            _subscriptions.Add(new EventSubscription(handler, priority));
        }

        public void Remove(object handler)
        {
            _subscriptions.Remove(new EventSubscription(handler));
        }

        public override void Invoke<TSignal>()
        {
            Invoke(default(TSignal));
        }

        public override void Invoke<TSignal>(TSignal signal)
        {
            if (_changed)
            {
                _subscriptions.Sort();
                _changed = false;
            }

            for (int i = 0; i < _subscriptions.Count; i++)
            {
                _subscriptions[i].Invoke(signal);
            }
        }
    }
}
