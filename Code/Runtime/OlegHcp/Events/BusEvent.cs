using System;
using System.Collections.Generic;
using System.Linq;
using OlegHcp.CSharp.Collections;

namespace OlegHcp.Events
{
    public abstract class BusEvent
    {
        public abstract void Invoke<TSignal>(TSignal signal) where TSignal : ISignal;
        public abstract void UnregisterOwner();
    }

    internal class InternalEvent : BusEvent
    {
        private HashSet<EventSubscription> _callbacks = new HashSet<EventSubscription>();
        private object _owner;

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

        public void Add<TSignal>(Action<TSignal> callback, int priority) where TSignal : ISignal
        {
            _callbacks.Add(new EventSubscription(callback, priority));
        }

        public void Remove<TSignal>(Action<TSignal> callback) where TSignal : ISignal
        {
            _callbacks.Remove(new EventSubscription(callback));
        }

        public override void Invoke<TSignal>(TSignal signal)
        {
            _callbacks.OrderBy(item => item.Priority)
                      .ForEach(item => item.Invoke(signal));
        }
    }
}
