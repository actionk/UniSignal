using System;

namespace Plugins.UniSignal.Subscriptions
{
    internal class SignalSubscriptionAnonymousConditionalWithData<T> : SignalSubscription<T> where T : struct, ISignal
    {
        private readonly Predicate<T> m_predicate;
        private readonly Action<T> m_callback;

        public SignalSubscriptionAnonymousConditionalWithData(Predicate<T> predicate, Action<T> callback, object listener = default)
        {
            m_predicate = predicate;
            m_callback = callback;
            Listener = listener;
        }

        public override ISignal Signal => default;
        public override Type SignalType => typeof(T);
        public override bool IsAnonymous => true;

        public override SignalSubscription<T> Trigger(T data)
        {
            if (!m_predicate.Invoke(data))
                return this;

            m_callback.Invoke(data);
            return this;
        }
    }
}