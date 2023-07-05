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
            public SignalSubscription subscription;
            public DelayedActionType actionType;

            public DelayedAction(SignalSubscription subscription, DelayedActionType actionType)
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
        private bool m_isLocked;
        private bool m_isDirty;

        private readonly MultiValueDictionaryList<ISignal, SignalSubscription> m_subscriptionsBySignal = new();
        private readonly List<SignalSubscription> m_anonymousSubscriptions = new();

        public void Subscribe(SignalSubscription subscription)
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

        private void ForceSubscribe(SignalSubscription subscription)
        {
            subscription.Storage = this;
            if (subscription.IsAnonymous)
                m_anonymousSubscriptions.Add(subscription);
            else
                m_subscriptionsBySignal.Add(subscription.Signal, subscription);
        }

        public void Unsubscribe(SignalSubscription subscription)
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

        private void ForceUnsubscribe(SignalSubscription subscription)
        {
            subscription.Storage = null;
            
            if (subscription.IsAnonymous)
                m_anonymousSubscriptions.Remove(subscription);
            else
                m_subscriptionsBySignal.Remove(subscription.Signal, subscription);
        }

        private void ForceUnsubscribeAll()
        {
            m_anonymousSubscriptions.Clear();
            m_subscriptionsBySignal.Clear();
        }

        public void Dispatch(ISignal signal)
        {
            Dispatch((T)signal);
        }

        public void Dispatch(T signal)
        {
            if (m_isDirty && !m_isLocked)
                ProcessDelayedActions();

            if (m_anonymousSubscriptions.Count > 0)
            {
                foreach (var subscription in m_anonymousSubscriptions)
                    subscription.Trigger(signal);
            }

            if (m_subscriptionsBySignal.Count > 0 && m_subscriptionsBySignal.TryGetValue(signal, out List<SignalSubscription> subscriptions))
            {
                foreach (var subscription in subscriptions)
                    subscription.Trigger(signal);
            }
        }
        
        private void ProcessDelayedActions()
        {
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

            m_delayedActions.Clear();
            m_isDirty = false;
        }
    }
}