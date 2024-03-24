using System;
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;

namespace OlegHcp.Events
{
    public interface ISignal
    {

    }

    public class SignalBus
    {
        private Dictionary<Type, BusEvent> _storage = new Dictionary<Type, BusEvent>();

        public void Subscribe<T>(Action<T> callback) where T : ISignal
        {
            Subscribe(int.MaxValue, callback);
        }

        public void Subscribe<T>(int priority, Action<T> callback) where T : ISignal
        {
            if (callback == null)
                throw new ArgumentNullException();

            GetEvent(typeof(T)).Add(callback, priority);
        }

        public void Unsubscribe<T>(Action<T> callback) where T : ISignal
        {
            if (callback == null)
                throw new ArgumentNullException();

            if (_storage.TryGetValue(typeof(T), out BusEvent @event))
                @event.Remove(callback);
        }

        public IEvent RegisterEventOwner<T>(object owner) where T : ISignal
        {
            if (owner == null)
                throw new ArgumentNullException();

            BusEvent @event = GetEvent(typeof(T));

            if (@event.TrySetOwner(owner))
                return @event;

            throw new Exception();
        }

        private BusEvent GetEvent(Type type)
        {
            if (_storage.TryGetValue(type, out BusEvent @event))
                return @event;

            return _storage.Place(type, new BusEvent(type));
        }
    }
}
