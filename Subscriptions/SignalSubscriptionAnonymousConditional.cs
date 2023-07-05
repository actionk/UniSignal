using System;

namespace Plugins.Polymorphex.Packages.UniSignal.Subscriptions
{
    internal class SignalSubscriptionAnonymousConditional<T> : ASignalSubscription where T : unmanaged, ISignal
    {
        private readonly Predicate<T> m_predicate;
        private readonly Action m_callback;

        public SignalSubscriptionAnonymousConditional(Predicate<T> predicate, Action callback, object listener = default)
        {
            m_predicate = predicate;
            m_callback = callback;
            Listener = listener;
        }

        public override ISignal Signal => default;
        public override Type SignalType => typeof(T);
        public override bool IsAnonymous => true;

        public override ASignalSubscription Trigger(ISignal data = default)
        {
            var dataOfTypeT = (T)data!;
            if (!m_predicate.Invoke(dataOfTypeT))
                return this;

            m_callback.Invoke();
            return this;
        }
    }
}