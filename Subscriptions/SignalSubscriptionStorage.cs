using System.Collections.Generic;
using Plugins.UniSignal.Utils;

namespace Plugins.UniSignal.Subscriptions
{
    internal class SignalSubscriptionStorage<T> : ISignalSubscriptionStorage where T : struct, ISignal
    {
        private enum DelayedActionType
        {
            SUBSCRIBE,
            UNSUBSCRIBE,
            UNSUBSCRIBE_ALL
        }

        private struct DelayedAction
        {
            public SignalSubscription<T> subscription;
            public DelayedActionType actionType;

            public DelayedAction(SignalSubscription<T> subscription, DelayedActionType actionType)
            {
                this.subscription = subscription;
                this.actionType = actionType;
            }

            public DelayedAction(DelayedActionType actionType) : this()
            {
                this.actionType = actionType;
            }
        }

        private readonly List<DelayedAction> m_delayedActions = new();
        private volatile bool m_isLocked;
        private volatile bool m_isDirty;

        private readonly MultiValueDictionaryList<ISignal, SignalSubscription<T>> m_subscriptionsBySignal = new();
        private readonly List<SignalSubscription<T>> m_anonymousSubscriptions = new();

        public void Subscribe(SignalSubscription<T> subscription)
        {
            if (m_isDirty && !m_isLocked)
                ProcessDelayedActions();

            if (m_isLocked)
            {
                m_delayedActions.Add(new DelayedAction(subscription, DelayedActionType.SUBSCRIBE));
                m_isDirty = true;
            }
            else
                ForceSubscribe(subscription);
        }

        public void Subscribe(ISignalSubscription subscription)
        {
            Subscribe((SignalSubscription<T>)subscription);
        }

        public void Unsubscribe(ISignalSubscription subscription)
        {
            Unsubscribe((SignalSubscription<T>)subscription);
        }

        private void ForceSubscribe(SignalSubscription<T> subscription)
        {
            subscription.Storage = this;

            m_isLocked = true;
            if (subscription.IsAnonymous)
                m_anonymousSubscriptions.Add(subscription);
            else
                m_subscriptionsBySignal.Add(subscription.Signal, subscription);
            m_isLocked = false;
        }

        public void Unsubscribe(SignalSubscription<T> subscription)
        {
            if (m_isDirty && !m_isLocked)
                ProcessDelayedActions();

            if (m_isLocked)
            {
                m_delayedActions.Add(new DelayedAction(subscription, DelayedActionType.UNSUBSCRIBE));
                m_isDirty = true;
            }
            else
                ForceUnsubscribe(subscription);
        }

        public void UnsubscribeAll()
        {
            if (m_isDirty && !m_isLocked)
                ProcessDelayedActions();

            if (m_isLocked)
            {
                m_delayedActions.Add(new DelayedAction(DelayedActionType.UNSUBSCRIBE_ALL));
                m_isDirty = true;
            }
            else
                ForceUnsubscribeAll();
        }

        public void Dispatch(T signal)
        {
            if (m_isDirty && !m_isLocked)
                ProcessDelayedActions();

            m_isLocked = true;
            if (m_anonymousSubscriptions.Count > 0)
            {
                foreach (var subscription in m_anonymousSubscriptions)
                    subscription.Trigger(signal);
            }

            if (m_subscriptionsBySignal.Count > 0 && m_subscriptionsBySignal.TryGetValue(signal, out List<SignalSubscription<T>> subscriptions))
            {
                foreach (var subscription in subscriptions)
                    subscription.Trigger(signal);
            }

            m_isLocked = false;
        }

        public void DispatchAsync(T signal)
        {
        }

        private void ForceUnsubscribe(SignalSubscription<T> subscription)
        {
            subscription.Storage = null;

            m_isLocked = true;
            if (subscription.IsAnonymous)
                m_anonymousSubscriptions.Remove(subscription);
            else
                m_subscriptionsBySignal.Remove(subscription.Signal, subscription);
            m_isLocked = false;
        }

        private void ForceUnsubscribeAll()
        {
            m_isLocked = true;
            m_anonymousSubscriptions.Clear();
            m_subscriptionsBySignal.Clear();
            m_isLocked = false;
        }

        private void ProcessDelayedActions()
        {
            m_isLocked = true;
            foreach (var delayedAction in m_delayedActions)
            {
                switch (delayedAction.actionType)
                {
                    case DelayedActionType.SUBSCRIBE:
                        ForceSubscribe(delayedAction.subscription);
                        break;

                    case DelayedActionType.UNSUBSCRIBE:
                        ForceUnsubscribe(delayedAction.subscription);
                        break;

                    case DelayedActionType.UNSUBSCRIBE_ALL:
                        ForceUnsubscribeAll();
                        break;
                }
            }

            m_isLocked = false;
            m_delayedActions.Clear();
            m_isDirty = false;
        }
    }
}