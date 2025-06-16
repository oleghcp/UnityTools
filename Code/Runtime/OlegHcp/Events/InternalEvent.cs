using System;
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;

namespace OlegHcp.Events
{
    internal class InternalEvent : BusEvent
    {
        private List<EventSubscription> _subscriptions = new List<EventSubscription>();
        private List<EventSubscription> _newItems;
        private List<int> _deadItems;
        private bool _changed;
        private bool _enumLocked;
        private bool _ownerLocked;

        public override Type SignalType { get; }
        public override bool Locked => _ownerLocked;

        public InternalEvent(Type signalType)
        {
            SignalType = signalType;
        }

        public bool TryLockToOwner()
        {
            if (_ownerLocked)
                return false;

            _ownerLocked = true;
            return true;
        }

        public override void Unlock()
        {
            _ownerLocked = false;
        }

        public int Subscribe(object handler, int priority)
        {
            EventSubscription s = new EventSubscription(handler, priority);

            if (_enumLocked)
            {
                if (_newItems == null)
                    _newItems = new List<EventSubscription>();
                _newItems.Add(s);
            }
            else
            {
                AddItem(s);
            }

            return s.GetHashCode();
        }

        public void Unsubscribe(int hash)
        {
            if (_enumLocked)
            {
                if (_deadItems == null)
                    _deadItems = new List<int>();
                _deadItems.Add(hash);
            }
            else
            {
                RemoveItem(hash);
            }
        }

        public override void Invoke<TSignal>(TSignal signal)
        {
            if (typeof(TSignal) != SignalType)
                throw new ArgumentException("Wrong signal type.");

            _enumLocked = true;

            if (_changed)
            {
                _subscriptions.Sort();
                _changed = false;
            }

            try
            {
                for (int i = 0; i < _subscriptions.Count; i++)
                {
                    _subscriptions[i].Invoke(signal);
                }
            }
            finally
            {
                _enumLocked = false;

                while (_deadItems.HasAnyData())
                {
                    RemoveItem(_deadItems.Pop());
                }

                while (_newItems.HasAnyData())
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
