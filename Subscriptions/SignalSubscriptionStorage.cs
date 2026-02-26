using System.Collections.Generic;
using UniSignal;
using UniSignal.Utils;

namespace UniSignal.Subscriptions
{
    internal class SignalSubscriptionStorage<T> : ISignalSubscriptionStorage where T : struct, ISignal
    {
        private enum DelayedActionType : byte
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

        private readonly List<DelayedAction> m_delayedActions = new(8);
        private readonly List<SignalSubscription<T>> m_subscriptionsToUnsubscribe = new(8);
        private readonly MultiValueDictionaryList<ISignal, SignalSubscription<T>> m_subscriptionsBySignal = new();
        private readonly List<SignalSubscription<T>> m_anonymousSubscriptions = new(8);

        private int m_dispatchDepth;
        private bool m_isDirty;

        public void Subscribe(SignalSubscription<T> subscription)
        {
            if (m_isDirty && m_dispatchDepth == 0)
                ProcessDelayedActions();

            if (m_dispatchDepth > 0)
            {
                m_delayedActions.Add(new DelayedAction(subscription, DelayedActionType.SUBSCRIBE));
                m_isDirty = true;
            }
            else
            {
                ForceSubscribe(subscription);
            }
        }

        public void Subscribe(ISignalSubscription subscription)
        {
            Subscribe((SignalSubscription<T>)subscription);
        }

        public void Unsubscribe(ISignalSubscription subscription)
        {
            Unsubscribe((SignalSubscription<T>)subscription);
        }

        public void Unsubscribe(SignalSubscription<T> subscription)
        {
            if (m_isDirty && m_dispatchDepth == 0)
                ProcessDelayedActions();

            if (m_dispatchDepth > 0)
            {
                m_delayedActions.Add(new DelayedAction(subscription, DelayedActionType.UNSUBSCRIBE));
                m_isDirty = true;
            }
            else
            {
                ForceUnsubscribe(subscription);
            }
        }

        public void UnsubscribeAll()
        {
            if (m_isDirty && m_dispatchDepth == 0)
                ProcessDelayedActions();

            if (m_dispatchDepth > 0)
            {
                m_delayedActions.Add(new DelayedAction(DelayedActionType.UNSUBSCRIBE_ALL));
                m_isDirty = true;
            }
            else
            {
                ForceUnsubscribeAll();
            }
        }

        public void Unsubscribe(object listener)
        {
            if (m_isDirty && m_dispatchDepth == 0)
                ProcessDelayedActions();

            m_subscriptionsToUnsubscribe.Clear();

            foreach (var kvp in m_subscriptionsBySignal)
            {
                var list = kvp.Value;
                for (int i = 0; i < list.Count; i++)
                {
                    var subscription = list[i];
                    if (ReferenceEquals(subscription.Listener, listener) ||
                        (subscription.Listener != null && subscription.Listener.Equals(listener)))
                    {
                        m_subscriptionsToUnsubscribe.Add(subscription);
                    }
                }
            }

            for (int i = 0; i < m_anonymousSubscriptions.Count; i++)
            {
                var subscription = m_anonymousSubscriptions[i];
                if (ReferenceEquals(subscription.Listener, listener) ||
                    (subscription.Listener != null && subscription.Listener.Equals(listener)))
                {
                    m_subscriptionsToUnsubscribe.Add(subscription);
                }
            }

            if (m_subscriptionsToUnsubscribe.Count == 0)
                return;

            if (m_dispatchDepth > 0)
            {
                for (int i = 0; i < m_subscriptionsToUnsubscribe.Count; i++)
                    m_delayedActions.Add(new DelayedAction(m_subscriptionsToUnsubscribe[i], DelayedActionType.UNSUBSCRIBE));

                m_isDirty = true;
            }
            else
            {
                for (int i = 0; i < m_subscriptionsToUnsubscribe.Count; i++)
                    ForceUnsubscribe(m_subscriptionsToUnsubscribe[i]);
            }
        }

        public void Dispatch(T signal)
        {
            if (m_isDirty && m_dispatchDepth == 0)
                ProcessDelayedActions();

            m_dispatchDepth++;

            // Dispatch to anonymous subscriptions (listen to all signals of type T)
            var anonCount = m_anonymousSubscriptions.Count;
            for (int i = 0; i < anonCount; i++)
                m_anonymousSubscriptions[i].Trigger(signal);

            // Dispatch to specific signal subscriptions
            if (m_subscriptionsBySignal.TryGetValue(signal, out var subscriptions))
            {
                var count = subscriptions.Count;
                for (int i = 0; i < count; i++)
                    subscriptions[i].Trigger(signal);
            }

            m_dispatchDepth--;

            // Process any pending actions after dispatch completes
            if (m_dispatchDepth == 0 && m_isDirty)
                ProcessDelayedActions();
        }

        private void ForceSubscribe(SignalSubscription<T> subscription)
        {
            subscription.Storage = this;

            if (subscription.IsAnonymous)
                m_anonymousSubscriptions.Add(subscription);
            else
                m_subscriptionsBySignal.Add(subscription.Signal, subscription);
        }

        private void ForceUnsubscribe(SignalSubscription<T> subscription)
        {
            if (subscription.IsAnonymous)
                m_anonymousSubscriptions.Remove(subscription);
            else
                m_subscriptionsBySignal.Remove(subscription.Signal, subscription);

            // Return subscription to pool
            var returnToPool = subscription.ReturnToPool;
            subscription.Reset();
            returnToPool?.Invoke(subscription);
        }

        private void ForceUnsubscribeAll()
        {
            // Return all anonymous subscriptions to pool
            for (int i = 0; i < m_anonymousSubscriptions.Count; i++)
            {
                var subscription = m_anonymousSubscriptions[i];
                var returnToPool = subscription.ReturnToPool;
                subscription.Reset();
                returnToPool?.Invoke(subscription);
            }

            // Return all specific subscriptions to pool
            foreach (var kvp in m_subscriptionsBySignal)
            {
                var list = kvp.Value;
                for (int i = 0; i < list.Count; i++)
                {
                    var subscription = list[i];
                    var returnToPool = subscription.ReturnToPool;
                    subscription.Reset();
                    returnToPool?.Invoke(subscription);
                }
            }

            m_anonymousSubscriptions.Clear();
            m_subscriptionsBySignal.Clear();
        }

        private void ProcessDelayedActions()
        {
            var count = m_delayedActions.Count;
            for (int i = 0; i < count; i++)
            {
                var delayedAction = m_delayedActions[i];
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

            m_delayedActions.Clear();
            m_isDirty = false;
        }
    }
}
