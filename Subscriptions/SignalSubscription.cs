using System;
using UniSignal;

namespace UniSignal.Subscriptions
{
    public abstract class SignalSubscription<T> : ISignalSubscription
        where T : struct, ISignal
    {
        internal ISignalSubscriptionStorage Storage { get; set; }
        internal Action<SignalSubscription<T>> ReturnToPool { get; set; }

        public object Listener { get; protected set; }
        public abstract ISignal Signal { get; }
        public abstract Type SignalType { get; }
        public abstract bool IsAnonymous { get; }

        public abstract SignalSubscription<T> Trigger(T data);
        public abstract void Reset();

        public void Unsubscribe()
        {
            Storage?.Unsubscribe(this);
        }
    }
}
