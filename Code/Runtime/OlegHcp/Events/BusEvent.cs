using System;
using System.Collections.Generic;
using System.Linq;
using OlegHcp.CSharp.Collections;

namespace OlegHcp.Events
{
    public interface IEvent
    {
        void Invoke<T>(T signal) where T : ISignal;
        void UnregisterOwner();
    }

    internal class BusEvent : IEvent
    {
        private HashSet<EventSubscription> _callbacks = new HashSet<EventSubscription>();
        private object _owner;

        public Type SignalType { get; }

        public BusEvent(Type signalType)
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

        public void UnregisterOwner()
        {
            _owner = null;
        }

        public void Add<T>(Action<T> callback, int priority) where T : ISignal
        {
            _callbacks.Add(new EventSubscription(callback, priority));
        }

        public void Remove<T>(Action<T> callback) where T : ISignal
        {
            _callbacks.Remove(new EventSubscription(callback));
        }

        public void Invoke<T>(T signal) where T : ISignal
        {
            _callbacks.OrderBy(item => item.Priority)
                      .ForEach(item => item.Invoke(signal));
        }
    }
}
