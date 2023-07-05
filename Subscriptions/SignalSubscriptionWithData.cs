using System;

namespace Plugins.Polymorphex.Packages.UniSignal.Subscriptions
{
    internal class SignalSubscriptionWithData<T> : ASignalSubscription where T : unmanaged, ISignal
    {
        private readonly T m_signal;
        private readonly Action<T> m_callback;

        public SignalSubscriptionWithData(T signal, Action<T> callback, object listener = default)
        {
            m_signal = signal;
            m_callback = callback;
            Listener = listener;
        }

        public override ISignal Signal => m_signal;
        public override Type SignalType => typeof(T);
        public override bool IsAnonymous => false;

        public override ASignalSubscription Trigger(ISignal data = default)
        {
            m_callback.Invoke((T)data!);
            return this;
        }
    }
}