using System;

namespace Plugins.Polymorphex.Packages.UniSignal.Subscriptions
{
    internal class SignalSubscription<T> : ASignalSubscription where T : unmanaged, ISignal
    {
        private readonly T m_signal;
        private readonly Action m_callback;

        public SignalSubscription(T signal, Action callback, object listener = default)
        {
            m_signal = signal;
            m_callback = callback;
            Listener = listener;
        }

        public override ISignal Signal => m_signal;
        public override Type SignalType => typeof(T);
        public override bool IsAnonymous => false;

        public override ASignalSubscription Trigger(ISignal data = default)
        {
            m_callback.Invoke();
            return this;
        }
    }
}