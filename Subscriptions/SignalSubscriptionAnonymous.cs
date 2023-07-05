using System;

namespace Plugins.Polymorphex.Packages.UniSignal.Subscriptions
{
    internal class SignalSubscriptionAnonymous<T> : ASignalSubscription where T : unmanaged, ISignal
    {
        private readonly Action m_callback;

        public SignalSubscriptionAnonymous(Action callback, object listener = default)
        {
            m_callback = callback;
            Listener = listener;
        }

        public override ISignal Signal => default;
        public override Type SignalType => typeof(T);
        public override bool IsAnonymous => true;

        public override ASignalSubscription Trigger(ISignal data = default)
        {
            m_callback.Invoke();
            return this;
        }
    }
}