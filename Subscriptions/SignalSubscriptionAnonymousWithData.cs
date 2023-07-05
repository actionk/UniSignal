using System;

namespace Plugins.Polymorphex.Packages.UniSignal.Subscriptions
{
    internal class SignalSubscriptionAnonymousWithData<T> : ASignalSubscription where T : unmanaged, ISignal
    {
        private readonly Action<T> m_callback;

        public SignalSubscriptionAnonymousWithData(Action<T> callback, object listener = default)
        {
            m_callback = callback;
            Listener = listener;
        }

        public override ISignal Signal => default;
        public override Type SignalType => typeof(T);
        public override bool IsAnonymous => true;

        public override ASignalSubscription Trigger(ISignal data = default)
        {
            m_callback.Invoke((T)data!);
            return this;
        }
    }
}