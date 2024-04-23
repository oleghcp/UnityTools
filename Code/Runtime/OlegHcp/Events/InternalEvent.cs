using System;
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;

namespace OlegHcp.Events
{
    internal class InternalEvent : BusEvent
    {
        private List<EventSubscription> _subscriptions = new List<EventSubscription>();

        private object _owner;
        private bool _changed;

        public Type SignalType { get; }
        public object Owner => _owner;
        public override bool HasOwner => _owner != null;

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

        public int Subscribe(object handler, int priority)
        {
            if (priority != int.MaxValue)
                _changed = true;

            EventSubscription s = new EventSubscription(handler, priority);
            _subscriptions.Add(s);
            return s.GetHashCode();
        }

        public void Unsubscribe(int hash)
        {
            int index = _subscriptions.IndexOf(item => item.GetHashCode() == hash);
    
            if (index >= 0)
                _subscriptions.RemoveAt(index);
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
