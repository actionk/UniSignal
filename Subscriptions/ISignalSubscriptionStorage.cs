namespace Plugins.Polymorphex.Packages.UniSignal.Subscriptions
{
    internal interface ISignalSubscriptionStorage
    {
        public void Subscribe(ASignalSubscription subscription);
        public void Unsubscribe(ASignalSubscription subscription);
        public void Dispatch(ISignal signal);
        void UnsubscribeAll();
    }
}