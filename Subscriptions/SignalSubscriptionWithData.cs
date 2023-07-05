using System;

namespace Plugins.UniSignal.Subscriptions
{
    internal class SignalSubscriptionWithData<T> : SignalSubscription where T : struct, ISignal
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

        public override SignalSubscription Trigger(ISignal data = default)
        {
            m_callback.Invoke((T)data!);
            return this;
        }
    }
}