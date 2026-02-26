using System;
using System.Collections.Generic;
using UniSignal.Subscriptions;
using UniSignal.Utils;

namespace UniSignal
{
    public class SignalHub
    {
        #region Subscription Pools

        private static class SubscriptionPools<T> where T : struct, ISignal
        {
            public static readonly ObjectPool<SignalSubscriptionAnonymous<T>> Anonymous = new();
            public static readonly ObjectPool<SignalSubscriptionAnonymousWithData<T>> AnonymousWithData = new();
            public static readonly ObjectPool<SignalSubscriptionAnonymousConditional<T>> AnonymousConditional = new();
            public static readonly ObjectPool<SignalSubscriptionAnonymousConditionalWithData<T>> AnonymousConditionalWithData = new();
            public static readonly ObjectPool<SignalSubscriptionSpecific<T>> Specific = new();
            public static readonly ObjectPool<SignalSubscriptionSpecificWithData<T>> SpecificWithData = new();
        }

        #endregion

        #region API

        public SignalSubscription<T> Subscribe<T>(object listener, Action callback) where T : struct, ISignal
        {
            var subscription = SubscriptionPools<T>.Anonymous.Get();
            subscription.Initialize(callback, listener);
            subscription.ReturnToPool = s => SubscriptionPools<T>.Anonymous.Return((SignalSubscriptionAnonymous<T>)s);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(Action callback) where T : struct, ISignal
        {
            var subscription = SubscriptionPools<T>.Anonymous.Get();
            subscription.Initialize(callback, null);
            subscription.ReturnToPool = s => SubscriptionPools<T>.Anonymous.Return((SignalSubscriptionAnonymous<T>)s);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(object listener, Action<T> callback) where T : struct, ISignal
        {
            var subscription = SubscriptionPools<T>.AnonymousWithData.Get();
            subscription.Initialize(callback, listener);
            subscription.ReturnToPool = s => SubscriptionPools<T>.AnonymousWithData.Return((SignalSubscriptionAnonymousWithData<T>)s);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(Action<T> callback) where T : struct, ISignal
        {
            var subscription = SubscriptionPools<T>.AnonymousWithData.Get();
            subscription.Initialize(callback, null);
            subscription.ReturnToPool = s => SubscriptionPools<T>.AnonymousWithData.Return((SignalSubscriptionAnonymousWithData<T>)s);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(object listener, Predicate<T> predicate, Action callback) where T : struct, ISignal
        {
            var subscription = SubscriptionPools<T>.AnonymousConditional.Get();
            subscription.Initialize(predicate, callback, listener);
            subscription.ReturnToPool = s => SubscriptionPools<T>.AnonymousConditional.Return((SignalSubscriptionAnonymousConditional<T>)s);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(Predicate<T> predicate, Action callback) where T : struct, ISignal
        {
            var subscription = SubscriptionPools<T>.AnonymousConditional.Get();
            subscription.Initialize(predicate, callback, null);
            subscription.ReturnToPool = s => SubscriptionPools<T>.AnonymousConditional.Return((SignalSubscriptionAnonymousConditional<T>)s);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(object listener, Predicate<T> predicate, Action<T> callback) where T : struct, ISignal
        {
            var subscription = SubscriptionPools<T>.AnonymousConditionalWithData.Get();
            subscription.Initialize(predicate, callback, listener);
            subscription.ReturnToPool = s => SubscriptionPools<T>.AnonymousConditionalWithData.Return((SignalSubscriptionAnonymousConditionalWithData<T>)s);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(Predicate<T> predicate, Action<T> callback) where T : struct, ISignal
        {
            var subscription = SubscriptionPools<T>.AnonymousConditionalWithData.Get();
            subscription.Initialize(predicate, callback, null);
            subscription.ReturnToPool = s => SubscriptionPools<T>.AnonymousConditionalWithData.Return((SignalSubscriptionAnonymousConditionalWithData<T>)s);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(object listener, T signal, Action callback) where T : struct, ISignal<T>
        {
            var subscription = SubscriptionPools<T>.Specific.Get();
            subscription.Initialize(signal, callback, listener);
            subscription.ReturnToPool = s => SubscriptionPools<T>.Specific.Return((SignalSubscriptionSpecific<T>)s);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(T signal, Action callback) where T : struct, ISignal<T>
        {
            var subscription = SubscriptionPools<T>.Specific.Get();
            subscription.Initialize(signal, callback, null);
            subscription.ReturnToPool = s => SubscriptionPools<T>.Specific.Return((SignalSubscriptionSpecific<T>)s);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(object listener, T signal, Action<T> callback) where T : struct, ISignal<T>
        {
            var subscription = SubscriptionPools<T>.SpecificWithData.Get();
            subscription.Initialize(signal, callback, listener);
            subscription.ReturnToPool = s => SubscriptionPools<T>.SpecificWithData.Return((SignalSubscriptionSpecificWithData<T>)s);
            AddSubscriber(subscription);
            return subscription;
        }

        public SignalSubscription<T> Subscribe<T>(T signal, Action<T> callback) where T : struct, ISignal<T>
        {
            var subscription = SubscriptionPools<T>.SpecificWithData.Get();
            subscription.Initialize(signal, callback, null);
            subscription.ReturnToPool = s => SubscriptionPools<T>.SpecificWithData.Return((SignalSubscriptionSpecificWithData<T>)s);
            AddSubscriber(subscription);
            return subscription;
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

        public void Unsubscribe<T>(object listener) where T : struct, ISignal
        {
            if (m_storagesBySignalType.TryGetValue(typeof(T), out ISignalSubscriptionStorage storage))
                storage.Unsubscribe(listener);
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

            for (int i = 0; i < subscriptions.Count; i++)
            {
                var subscription = subscriptions[i];
                if (m_storagesBySignalType.TryGetValue(subscription.SignalType, out ISignalSubscriptionStorage storage))
                    storage.Unsubscribe(subscription);
            }
        }

        public IEnumerable<object> GetSubscriptionListeners()
        {
            return m_subscriptionsByListeners.Keys;
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
