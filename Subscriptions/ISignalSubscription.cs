using System;

namespace Plugins.UniSignal.Subscriptions
{
    public interface ISignalSubscription
    {
        public object Listener { get; }
        public abstract ISignal Signal { get; }
        public abstract Type SignalType { get; }
        public abstract bool IsAnonymous { get; }
    }
}