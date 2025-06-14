using System;

namespace OlegHcp.Events
{
    public class EventLockedException : InvalidOperationException
    {
        internal EventLockedException(InternalEvent @event)
            : base($"Event {@event.SignalType.Name} locked.")
        {

        }
    }

    public class InvalidLockException : InvalidOperationException
    {
        internal InvalidLockException(InternalEvent @event)
            : base($"Event {@event.SignalType.Name} already locked.")
        {

        }
    }
}
