using System;

namespace OlegHcp.Events
{
    public class OwnerRegisteredException : InvalidOperationException
    {
        private InternalEvent _event;

        public override string Message
        {
            get
            {
                if (_event is null)
                    return base.Message;

                return base.Message + Environment.NewLine + GetValueMessage(_event);
            }
        }

        internal OwnerRegisteredException(InternalEvent @event) : base(GetMessage())
        {
            _event = @event;
        }

        private static string GetMessage()
        {
            return $"Event already has owner.";
        }

        private static string GetValueMessage(InternalEvent @event)
        {
            return $"Owner is {@event.Owner.GetType().Name}.";
        }
    }
}
