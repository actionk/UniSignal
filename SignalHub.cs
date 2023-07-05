using System;
using System.Collections.Generic;
using Plugins.UniSignal.Subscriptions;
using Plugins.UniSignal.Utils;

namespace Plugins.UniSignal
{
    public class SignalHub
    {
#region API

        public SignalSubscription Subscribe<T>(object listener, Action callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymous<T>(callback, listener);
            AddSubscriber<T>(subscription);
            return subscription;
        }

        public SignalSubscription Subscribe<T>(Action callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymous<T>(callback);
            AddSubscriber<T>(subscription);
            return subscription;
        }

        public SignalSubscription Subscribe<T>(object listener, Action<T> callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymousWithData<T>(callback, listener);
            AddSubscriber<T>(subscription);
            return subscription;
        }

        public SignalSubscription Subscribe<T>(Action<T> callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymousWithData<T>(callback);
            AddSubscriber<T>(subscription);
            return subscription;
        }

        public SignalSubscription Subscribe<T>(object listener, Predicate<T> predicate, Action callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymousConditional<T>(predicate, callback, listener);
            AddSubscriber<T>(subscription);
            return subscription;
        }

        public SignalSubscription Subscribe<T>(Predicate<T> predicate, Action callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymousConditional<T>(predicate, callback);
            AddSubscriber<T>(subscription);
            return subscription;
        }

        public SignalSubscription Subscribe<T>(object listener, Predicate<T> predicate, Action<T> callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymousConditionalWithData<T>(predicate, callback, listener);
            AddSubscriber<T>(subscription);
            return subscription;
        }

        public SignalSubscription Subscribe<T>(Predicate<T> predicate, Action<T> callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymousConditionalWithData<T>(predicate, callback);
            AddSubscriber<T>(subscription);
            return subscription;
        }

        public SignalSubscription Subscribe<T>(object listener, T signal, Action callback) where T : struct, ISignal<T>
        {
            var subscription = new SignalSubscriptionSpecific<T>(signal, callback, listener);
            AddSubscriber<T>(subscription);
            return subscription;
        }

        public SignalSubscription Subscribe<T>(T signal, Action callback) where T : struct, ISignal<T>
        {
            var subscription = new SignalSubscriptionSpecific<T>(signal, callback);
            AddSubscriber<T>(subscription);
            return subscription;
        }

        public SignalSubscription Subscribe<T>(object listener, T signal, Action<T> callback) where T : struct, ISignal<T>
        {
            var subscription = new SignalSubscriptionSpecificWithData<T>(signal, callback, listener);
            AddSubscriber<T>(subscription);
            return subscription;
        }

        public SignalSubscription Subscribe<T>(T signal, Action<T> callback) where T : struct, ISignal<T>
        {
            var subscription = new SignalSubscriptionSpecificWithData<T>(signal, callback);
            AddSubscriber<T>(subscription);
            return subscription;
        }

        public void Dispatch<T>(T signal) where T : struct, ISignal
        {
            if (!m_storagesBySignalType.TryGetValue(typeof(T), out ISignalSubscriptionStorage storage))
                return;

            storage.Dispatch(signal);
        }

        public void UnsubscribeAllFrom<T>() where T : struct, ISignal
        {
            if (m_storagesBySignalType.TryGetValue(typeof(T), out ISignalSubscriptionStorage storage))
                storage.UnsubscribeAll();
        }

        public void Unsubscribe(SignalSubscription subscription)
        {
            if (m_storagesBySignalType.TryGetValue(subscription.SignalType, out ISignalSubscriptionStorage storage))
                storage.Unsubscribe(subscription);
        }

        public void Unsubscribe(object listener)
        {
            if (!m_subscriptionsByListeners.TryGetValue(listener, out List<SignalSubscription> subscriptions))
                return;

            foreach (var subscription in subscriptions)
            {
                if (m_storagesBySignalType.TryGetValue(subscription.SignalType, out ISignalSubscriptionStorage storage))
                    storage.Unsubscribe(subscription);
            }
        }

#endregion

#region Private

        private readonly Dictionary<Type, ISignalSubscriptionStorage> m_storagesBySignalType = new();
        private readonly MultiValueDictionaryList<object, SignalSubscription> m_subscriptionsByListeners = new();

        private void AddSubscriber<T>(SignalSubscription subscription) where T : struct, ISignal
        {
            var subscriptionSignalType = subscription.SignalType;
            if (!m_storagesBySignalType.TryGetValue(subscriptionSignalType, out ISignalSubscriptionStorage storage))
            {
                storage = new SignalSubscriptionStorage<T>();
                m_storagesBySignalType[subscriptionSignalType] = storage;
            }

            storage.Subscribe(subscription);
            m_subscriptionsByListeners.Add(subscription.Listener, subscription);
        }

#endregion
    }
}