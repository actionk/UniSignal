using System;
using UniSignal;

namespace UniSignal.Subscriptions
{
    internal class SignalSubscriptionAnonymous<T> : SignalSubscription<T> where T : struct, ISignal
    {
        private Action m_callback;

        public SignalSubscriptionAnonymous() { }

        public void Initialize(Action callback, object listener)
        {
            m_callback = callback;
            Listener = listener;
        }

        public override void Reset()
        {
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
            m_callback?.Invoke();
            return this;
        }
    }
}
