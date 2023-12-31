﻿using System;

namespace Plugins.UniSignal.Subscriptions
{
    internal class SignalSubscriptionSpecific<T> : SignalSubscription<T> where T : struct, ISignal
    {
        private readonly T m_signal;
        private readonly Action m_callback;

        public SignalSubscriptionSpecific(T signal, Action callback, object listener = default)
        {
            m_signal = signal;
            m_callback = callback;
            Listener = listener;
        }

        public override ISignal Signal => m_signal;
        public override Type SignalType => typeof(T);
        public override bool IsAnonymous => false;

        public override SignalSubscription<T> Trigger(T data)
        {
            m_callback.Invoke();
            return this;
        }
    }
}