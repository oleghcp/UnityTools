using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OlegHcp.CSharp.Collections;

namespace OlegHcp.Events
{
    internal class InternalEvent : BusEvent
    {
        private List<EventSubscription> _subscriptions = new List<EventSubscription>();
        private Queue<ChangeToken> _changes;
        private bool _orderRequired;
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
                _changes ??= new Queue<ChangeToken>();
                _changes.Enqueue(new ChangeToken(s));
            }
            else
            {
                AddItem(s);
            }

            return s.HashCode;
        }

        public void Unsubscribe(int hash)
        {
            if (_enumLocked)
            {
                _changes ??= new Queue<ChangeToken>();
                _changes.Enqueue(new ChangeToken(hash));
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

            if (_orderRequired)
            {
                _subscriptions.Sort();
                _orderRequired = false;
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

                if (_changes != null)
                {
                    while (_changes.Count > 0)
                    {
                        _changes.Dequeue()
                                .Proceed(this);
                    }
                }
            }
        }

        private void AddItem(in EventSubscription subscription)
        {
            if (subscription.Priority != int.MaxValue)
                _orderRequired = true;

            _subscriptions.Add(subscription);
        }

        private void RemoveItem(int hash)
        {
            int index = _subscriptions.IndexOf(item => item.HashCode == hash);

            if (index >= 0)
                _subscriptions.RemoveAt(index);
        }

        private struct ChangeToken
        {
            private EventSubscription _item;
            private ChangeActionType _actionType;

            public ChangeToken(EventSubscription item)
            {
                _item = item;
                _actionType = ChangeActionType.Add;
            }

            public ChangeToken(int hash)
            {
                _item = new EventSubscription(hash);
                _actionType = ChangeActionType.Remove;
            }

            public void Proceed(InternalEvent thisEvent)
            {
                switch (_actionType)
                {
                    case ChangeActionType.Add:
                        thisEvent.AddItem(_item);
                        break;

                    case ChangeActionType.Remove:
                        thisEvent.RemoveItem(_item.HashCode);
                        break;

                    default:
                        throw new SwitchExpressionException(_actionType);
                }
            }
        }

        private enum ChangeActionType
        {
            Add,
            Remove,
        }
    }
}
