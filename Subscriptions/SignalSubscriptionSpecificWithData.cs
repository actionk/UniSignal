using System;

namespace Plugins.UniSignal.Subscriptions
{
    internal class SignalSubscriptionSpecificWithData<T> : SignalSubscription<T> where T : struct, ISignal
    {
        private readonly T m_signal;
        private readonly Action<T> m_callback;

        public SignalSubscriptionSpecificWithData(T signal, Action<T> callback, object listener = default)
        {
            m_signal = signal;
            m_callback = callback;
            Listener = listener;
        }

        public override ISignal Signal => m_signal;
        public override Type SignalType => typeof(T);
        public override bool IsAnonymous => false;

        public override void Trigger(T data)
        {
            m_callback.Invoke(data);
        }
    }
}