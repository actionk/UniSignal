using System;

namespace Plugins.Polymorphex.Packages.UniSignal.Subscriptions
{
    public abstract class ASignalSubscription
    {
        internal ISignalSubscriptionStorage Storage { get; set; }
        
        public object Listener { get; protected set; }
        public abstract ISignal Signal { get; }
        public abstract Type SignalType { get; }
        public abstract bool IsAnonymous { get; }
        
        public abstract ASignalSubscription Trigger(ISignal data = default);

        public void Unsubscribe()
        {
            Storage?.Unsubscribe(this);
        }
    }
}