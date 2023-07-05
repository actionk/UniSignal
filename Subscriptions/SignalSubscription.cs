using System;

namespace Plugins.UniSignal.Subscriptions
{
    public abstract class SignalSubscription
    {
        internal ISignalSubscriptionStorage Storage { get; set; }

        public object Listener { get; protected set; }
        public abstract ISignal Signal { get; }
        public abstract Type SignalType { get; }
        public abstract bool IsAnonymous { get; }

        public abstract SignalSubscription Trigger(ISignal data = default);

        public void Unsubscribe()
        {
            Storage?.Unsubscribe(this);
        }
    }
}