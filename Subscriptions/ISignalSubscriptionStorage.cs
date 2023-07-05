namespace Plugins.UniSignal.Subscriptions
{
    internal interface ISignalSubscriptionStorage
    {
        public void Subscribe(SignalSubscription subscription);
        public void Unsubscribe(SignalSubscription subscription);
        public void Dispatch(ISignal signal);
        void UnsubscribeAll();
    }
}