using System;
using System.Collections.Generic;

namespace OlegHcp.Events
{
    internal class InternalEvent : BusEvent
    {
        private List<EventSubscription> _subscriptions = new List<EventSubscription>();
        private Stack<EventSubscription> _newItems;
        private Stack<int> _deadItems;
        private object _owner;
        private bool _changed;
        private bool _locked;

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
            EventSubscription s = new EventSubscription(handler, priority);

            if (_locked)
            {
                if (_newItems == null)
                    _newItems = new Stack<EventSubscription>();
                _newItems.Push(s);
            }
            else
            {
                AddItem(s);
            }

            return s.GetHashCode();
        }

        public void Unsubscribe(int hash)
        {
            if (_locked)
            {
                if (_deadItems == null)
                    _deadItems = new Stack<int>();
                _deadItems.Push(hash);
            }
            else
            {
                RemoveItem(hash);
            }
        }

        public override void Invoke<TSignal>(TSignal signal)
        {
            _locked = true;
            if (_changed)
            {
                _subscriptions.Sort();
                _changed = false;
            }

            for (int i = 0; i < _subscriptions.Count; i++)
            {
                _subscriptions[i].Invoke(signal);
            }
            _locked = false;

            if (_deadItems != null)
            {
                while (_deadItems.Count > 0)
                {
                    RemoveItem(_deadItems.Pop());
                }
            }

            if (_newItems != null)
            {
                while (_newItems.Count > 0)
                {
                    AddItem(_newItems.Pop());
                }
            }
        }

        private void AddItem(in EventSubscription subscription)
        {
            if (subscription.Priority != int.MaxValue)
                _changed = true;

            _subscriptions.Add(subscription);
        }

        private void RemoveItem(int hash)
        {
            int index = -1;

            for (int i = 0; i < _subscriptions.Count; i++)
            {
                EventSubscription s = _subscriptions[i];
                if (s.GetHashCode() == hash)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
                _subscriptions.RemoveAt(index);
        }
    }
}
