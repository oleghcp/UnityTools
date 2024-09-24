using System;

namespace OlegHcp.Events
{
    public class OwnerRegisteredException : InvalidOperationException
    {
        internal OwnerRegisteredException(InternalEvent @event)
            : base($"Event {@event.SignalType.Name} already has owner: {@event.Owner.GetType().Name}.")
        {

        }
    }
}
