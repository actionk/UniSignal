using System;
using UniSignal;

namespace UniSignal.Subscriptions
{
    internal class SignalSubscriptionSpecificWithData<T> : SignalSubscription<T> where T : struct, ISignal
    {
        private T m_signal;
        private Action<T> m_callback;

        public SignalSubscriptionSpecificWithData() { }

        public void Initialize(T signal, Action<T> callback, object listener)
        {
            m_signal = signal;
            m_callback = callback;
            Listener = listener;
        }

        public override void Reset()
        {
            m_signal = default;
            m_callback = null;
            Listener = null;
            Storage = null;
            ReturnToPool = null;
        }

        public override ISignal Signal => m_signal;
        public override Type SignalType => typeof(T);
        public override bool IsAnonymous => false;

        public override SignalSubscription<T> Trigger(T data)
        {
            m_callback?.Invoke(data);
            return this;
        }
    }
}
