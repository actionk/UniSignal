using System;

namespace Plugins.UniSignal.Subscriptions
{
    internal class SignalSubscriptionAnonymousWithData<T> : SignalSubscription<T> where T : struct, ISignal
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

        public override SignalSubscription<T> Trigger(T data)
        {
            m_callback.Invoke(data);
            return this;
        }
    }
}