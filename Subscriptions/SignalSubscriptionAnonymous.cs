using System;

namespace Plugins.UniSignal.Subscriptions
{
    internal class SignalSubscriptionAnonymous<T> : SignalSubscription<T> where T : struct, ISignal
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

        public override void Trigger(T data)
        {
            m_callback.Invoke();
        }
    }
}