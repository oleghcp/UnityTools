﻿using System;
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

        public BusEvent LockEvent<TSignal>() where TSignal : ISignal
        {
            InternalEvent @event = GetOrCreateEvent(typeof(TSignal));

            if (@event.TryLockToOwner())
                return @event;

            throw new InvalidLockException(@event);
        }

        public SubscriptionToken Subscribe<TSignal>(Action<TSignal> callback, int priority = int.MaxValue) where TSignal : ISignal
        {
            if (callback == null)
                throw ThrowErrors.NullParameter(nameof(callback));

            return SubscribeInternal(callback, typeof(TSignal), priority);
        }

        public SubscriptionToken Subscribe<TSignal>(IEventListener<TSignal> listener, int priority = int.MaxValue) where TSignal : ISignal
        {
            if (listener == null)
                throw ThrowErrors.NullParameter(nameof(listener));

            return SubscribeInternal(listener, typeof(TSignal), priority);
        }

        public void Unsubscribe<TSignal>(Action<TSignal> callback) where TSignal : ISignal
        {
            if (callback == null)
                throw ThrowErrors.NullParameter(nameof(callback));

            if (_storage.TryGetValue(typeof(TSignal), out InternalEvent @event))
                @event.Unsubscribe(callback.GetHashCode());
        }

        public void Unsubscribe<TSignal>(IEventListener<TSignal> listener) where TSignal : ISignal
        {
            if (listener == null)
                throw ThrowErrors.NullParameter(nameof(listener));

            if (_storage.TryGetValue(typeof(TSignal), out InternalEvent @event))
                @event.Unsubscribe(listener.GetHashCode());
        }

        public void Unsubscribe(SubscriptionToken token)
        {
            if (token.SignalType == null)
                return;

            if (_storage.TryGetValue(token.SignalType, out InternalEvent @event))
                @event.Unsubscribe(token.Hash);
        }

        public void Invoke<TSignal>(TSignal signal) where TSignal : ISignal
        {
            if (_storage.TryGetValue(typeof(TSignal), out InternalEvent @event))
            {
                if (@event.Locked)
                    throw new EventLockedException(@event);

                @event.Invoke(signal);
            }
        }

        public bool IsEventLocked<TSignal>(TSignal signal)
        {
            if (_storage.TryGetValue(typeof(TSignal), out InternalEvent @event))
                return @event.Locked;

            return false;
        }

        private SubscriptionToken SubscribeInternal(object handler, Type signalType, int priority)
        {
            InternalEvent @event = GetOrCreateEvent(signalType);
            int hash = @event.Subscribe(handler, priority);
            return new SubscriptionToken
            {
                Hash = hash,
                SignalType = signalType,
            };
        }

        private InternalEvent GetOrCreateEvent(Type signalType)
        {
            if (_storage.TryGetValue(signalType, out InternalEvent @event))
                return @event;

            return _storage.Place(signalType, new InternalEvent(signalType));
        }
    }
}
