using NUnit.Framework;

namespace Plugins.UniSignal.Tests
{
    public class Signals_Sync_Test
    {
        private struct TestSignal : ISignal
        {
        }

        [Test]
        public void Test_Signal_Locking_Worked()
        {
            var signalHub = new SignalHub();

            var isSignalReceived = false;
            var isSecondSubscriptionWorked = false;

            signalHub.Subscribe<TestSignal>(this, () =>
            {
                if (!isSignalReceived)
                    signalHub.Subscribe<TestSignal>(this, () => isSecondSubscriptionWorked = true);

                isSignalReceived = true;
            });
            signalHub.Dispatch(new TestSignal());

            Assert.IsTrue(isSignalReceived);
            Assert.IsFalse(isSecondSubscriptionWorked);

            signalHub.Dispatch(new TestSignal());
            Assert.IsTrue(isSecondSubscriptionWorked);
        }
    }
}