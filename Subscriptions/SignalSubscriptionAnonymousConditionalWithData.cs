using System;
using UniSignal;

namespace UniSignal.Subscriptions
{
    internal class SignalSubscriptionAnonymousConditionalWithData<T> : SignalSubscription<T> where T : struct, ISignal
    {
        private Predicate<T> m_predicate;
        private Action<T> m_callback;

        public SignalSubscriptionAnonymousConditionalWithData() { }

        public void Initialize(Predicate<T> predicate, Action<T> callback, object listener)
        {
            m_predicate = predicate;
            m_callback = callback;
            Listener = listener;
        }

        public override void Reset()
        {
            m_predicate = null;
            m_callback = null;
            Listener = null;
            Storage = null;
            ReturnToPool = null;
        }

        public override ISignal Signal => default;
        public override Type SignalType => typeof(T);
        public override bool IsAnonymous => true;

        public override SignalSubscription<T> Trigger(T data)
        {
            if (m_predicate != null && !m_predicate.Invoke(data))
                return this;

            m_callback?.Invoke(data);
            return this;
        }
    }
}
