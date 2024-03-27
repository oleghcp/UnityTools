using System;
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;
using OlegHcp.Tools;

namespace OlegHcp.Events
{
    public interface ISignal { }

    public interface IEventListener<TSignal> where TSignal : ISignal
    {
        void HandleEvent(TSignal signal);
    }

    public class SignalBus
    {
        private Dictionary<Type, InternalEvent> _storage = new Dictionary<Type, InternalEvent>();

        public void Subscribe<TSignal>(Action<TSignal> callback) where TSignal : ISignal
        {
            Subscribe(int.MaxValue, callback);
        }

        public void Subscribe<TSignal>(int priority, Action<TSignal> callback) where TSignal : ISignal
        {
            if (callback == null)
                throw ThrowErrors.NullParameter(nameof(callback));

            GetEvent(typeof(TSignal)).Add(callback, priority);
        }

        public void Subscribe<TSignal>(IEventListener<TSignal> listener) where TSignal : ISignal
        {
            Subscribe(int.MaxValue, listener);
        }

        public void Subscribe<TSignal>(int priority, IEventListener<TSignal> listener) where TSignal : ISignal
        {
            if (listener == null)
                throw ThrowErrors.NullParameter(nameof(listener));

            GetEvent(typeof(TSignal)).Add(listener, priority);
        }

        public void Unsubscribe<TSignal>(Action<TSignal> callback) where TSignal : ISignal
        {
            if (callback == null)
                throw ThrowErrors.NullParameter(nameof(callback));

            if (_storage.TryGetValue(typeof(TSignal), out InternalEvent @event))
                @event.Remove(callback);
        }

        public void Unsubscribe<TSignal>(IEventListener<TSignal> listener) where TSignal : ISignal
        {
            if (listener == null)
                throw ThrowErrors.NullParameter(nameof(listener));

            if (_storage.TryGetValue(typeof(TSignal), out InternalEvent @event))
                @event.Remove(listener);
        }

        public BusEvent RegisterEventOwner<TSignal>(object owner) where TSignal : ISignal
        {
            if (owner == null)
                throw ThrowErrors.NullParameter(nameof(owner));

            InternalEvent @event = GetEvent(typeof(TSignal));

            if (@event.TrySetOwner(owner))
                return @event;

            throw new OwnerRegisteringException($"Event {@event.SignalType.Name} already has owner: {@event.Owner.GetType().Name}.");
        }

        private InternalEvent GetEvent(Type type)
        {
            if (_storage.TryGetValue(type, out InternalEvent @event))
                return @event;

            return _storage.Place(type, new InternalEvent(type));
        }
    }
}
