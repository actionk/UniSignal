using System;
using System.Collections.Generic;
using Plugins.UniSignal.Core;
using Plugins.UniSignal.Subscriptions;
using Plugins.UniSignal.Utils;

namespace Plugins.UniSignal
{
    public class SignalHub
    {
#region API

        public SignalSubscription<T> Subscribe<T>(object listener, Action callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymous<T>(callback, listener);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(Action callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymous<T>(callback);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(object listener, Action<T> callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymousWithData<T>(callback, listener);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(Action<T> callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymousWithData<T>(callback);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(object listener, Predicate<T> predicate, Action callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymousConditional<T>(predicate, callback, listener);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(Predicate<T> predicate, Action callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymousConditional<T>(predicate, callback);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(object listener, Predicate<T> predicate, Action<T> callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymousConditionalWithData<T>(predicate, callback, listener);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(Predicate<T> predicate, Action<T> callback) where T : struct, ISignal
        {
            var subscription = new SignalSubscriptionAnonymousConditionalWithData<T>(predicate, callback);
            AddSubscriber<T>(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(object listener, T signal, Action callback) where T : struct, ISignal<T>
        {
            var subscription = new SignalSubscriptionSpecific<T>(signal, callback, listener);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(T signal, Action callback) where T : struct, ISignal<T>
        {
            var subscription = new SignalSubscriptionSpecific<T>(signal, callback);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(object listener, T signal, Action<T> callback) where T : struct, ISignal<T>
        {
            var subscription = new SignalSubscriptionSpecificWithData<T>(signal, callback, listener);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(T signal, Action<T> callback) where T : struct, ISignal<T>
        {
            var subscription = new SignalSubscriptionSpecificWithData<T>(signal, callback);
            AddSubscriber(subscription);
            return subscription;
        }

        private AsyncSignalManager m_asyncSignalManager;

        public void SetCustomAsyncSignalManager(AsyncSignalManager asyncSignalManager)
        {
            m_asyncSignalManager = asyncSignalManager;
        }

        public void DispatchAsync<T>(T signal) where T : struct, ISignal
        {
            m_asyncSignalManager ??= AsyncSignalManager.Instance;
            m_asyncSignalManager.AddSignal(this, signal);
        }

        public void Dispatch<T>(T signal) where T : struct, ISignal
        {
            if (!TryGetSignalStorageOfType(typeof(T), out SignalSubscriptionStorage<T> storage))
                return;

            storage.Dispatch(signal);
        }

        public void UnsubscribeAllFrom<T>() where T : struct, ISignal
        {
            if (m_storagesBySignalType.TryGetValue(typeof(T), out ISignalSubscriptionStorage storage))
                storage.UnsubscribeAll();
        }

        public void Unsubscribe(ISignalSubscription subscription)
        {
            if (m_storagesBySignalType.TryGetValue(subscription.SignalType, out ISignalSubscriptionStorage storage))
                storage.Unsubscribe(subscription);
        }

        public void Unsubscribe(object listener)
        {
            if (!m_subscriptionsByListeners.TryGetValue(listener, out List<ISignalSubscription> subscriptions))
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
        private readonly MultiValueDictionaryList<object, ISignalSubscription> m_subscriptionsByListeners = new();

        private bool TryGetSignalStorageOfType<T>(Type type, out SignalSubscriptionStorage<T> storage) where T : struct, ISignal
        {
            if (!m_storagesBySignalType.TryGetValue(type, out ISignalSubscriptionStorage abstractStorage))
            {
                storage = default;
                return false;
            }

            storage = (SignalSubscriptionStorage<T>)abstractStorage;
            return true;
        }

        private void AddSubscriber<T>(SignalSubscription<T> subscription) where T : struct, ISignal
        {
            var subscriptionSignalType = subscription.SignalType;
            if (!TryGetSignalStorageOfType(subscriptionSignalType, out SignalSubscriptionStorage<T> storage))
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