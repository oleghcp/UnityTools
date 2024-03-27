using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using OlegHcp.CSharp.Collections;
using OlegHcp.Tools;

namespace OlegHcp.Events
{
    public interface ISignal { }

    public class SignalBus
    {
        private Dictionary<Type, InternalEvent> _storage = new Dictionary<Type, InternalEvent>();

        public void Subscribe<T>(Action<T> callback) where T : ISignal
        {
            Subscribe(int.MaxValue, callback);
        }

        public void Subscribe<T>(int priority, Action<T> callback) where T : ISignal
        {
            if (callback == null)
                throw ThrowErrors.NullParameter(nameof(callback));

            GetEvent(typeof(T)).Add(callback, priority);
        }

        public void Unsubscribe<T>(Action<T> callback) where T : ISignal
        {
            if (callback == null)
                throw ThrowErrors.NullParameter(nameof(callback));

            if (_storage.TryGetValue(typeof(T), out InternalEvent @event))
                @event.Remove(callback);
        }

        public BusEvent RegisterEventOwner<T>(object owner) where T : ISignal
        {
            if (owner == null)
                throw ThrowErrors.NullParameter(nameof(owner));

            InternalEvent @event = GetEvent(typeof(T));

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

    public class OwnerRegisteringException : Exception
    {
        public OwnerRegisteringException() : base() { }
        public OwnerRegisteringException(string message) : base(message) { }
        public OwnerRegisteringException(string message, Exception innerException) : base(message, innerException) { }
        public OwnerRegisteringException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
