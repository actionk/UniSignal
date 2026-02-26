using System;
using UniSignal;

namespace UniSignal.Subscriptions
{
    internal class SignalSubscriptionAnonymousWithData<T> : SignalSubscription<T> where T : struct, ISignal
    {
        private Action<T> m_callback;

        public SignalSubscriptionAnonymousWithData() { }

        public void Initialize(Action<T> callback, object listener)
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
            m_callback?.Invoke(data);
            return this;
        }
    }
}
