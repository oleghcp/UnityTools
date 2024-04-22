using System;

namespace OlegHcp.Events
{
    public struct SubscriptionToken
    {
        internal int Hash;
        internal Type SignalType;
    }
}
