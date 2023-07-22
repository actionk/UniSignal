namespace Plugins.UniSignal.Subscriptions
{
    internal interface ISignalSubscriptionStorage
    {
        public void Subscribe(ISignalSubscription subscription);
        public void Unsubscribe(ISignalSubscription subscription);
        public void Unsubscribe(object listener);

        void UnsubscribeAll();
    }
}