using System;

namespace Plugins.UniSignal.Subscriptions
{
    internal class SignalSubscriptionAnonymousWithData<T> : SignalSubscription where T : unmanaged, ISignal
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

        public override SignalSubscription Trigger(ISignal data = default)
        {
            m_callback.Invoke((T)data!);
            return this;
        }
    }
}