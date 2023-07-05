using System.Diagnostics;
using NUnit.Framework;
using Debug = UnityEngine.Debug;

namespace Plugins.UniSignal.Tests
{
    public class SignalsTest
    {
        private struct TestSignal : ISignal
        {
        }

        private struct TestSignalWithData : ISignal<TestSignalWithData>
        {
            public int testData;

            public TestSignalWithData(int testData)
            {
                this.testData = testData;
            }

            public bool Equals(TestSignalWithData other)
            {
                return testData == other.testData;
            }

            public override bool Equals(object obj)
            {
                return obj is TestSignalWithData other && Equals(other);
            }

            public override int GetHashCode()
            {
                return testData;
            }
        }

        [Test]
        public void Test_Signal_Empty()
        {
            var signalHub = new SignalHub();

            var isSignalReceived = false;

            signalHub.Subscribe<TestSignal>(this, () => isSignalReceived = true);
            signalHub.Dispatch(new TestSignal());

            Assert.IsTrue(isSignalReceived);
        }

        [Test]
        public void Test_Signal_Empty_Unsubscribe()
        {
            var signalHub = new SignalHub();

            var signalReceived = 0;

            signalHub.Subscribe<TestSignal>(this, () => signalReceived++);
            signalHub.Dispatch(new TestSignal());
            
            signalHub.Unsubscribe(this);
            signalHub.Dispatch(new TestSignal());
            
            Assert.AreEqual(1, signalReceived);
        }

        [Test]
        public void Test_Signal_Empty_Unsubscribe_SpecificSubscription()
        {
            var signalHub = new SignalHub();

            var signalReceived = 0;

            var subscription1 = signalHub.Subscribe<TestSignal>(this, () => signalReceived++);
            var subscription2 = signalHub.Subscribe<TestSignal>(this, () => signalReceived++);
            signalHub.Dispatch(new TestSignal());
            Assert.AreEqual(2, signalReceived);
            
            signalHub.Unsubscribe(subscription1);
            signalHub.Dispatch(new TestSignal());
            Assert.AreEqual(3, signalReceived);
            
            subscription2.Unsubscribe();
            signalHub.Dispatch(new TestSignal());
            Assert.AreEqual(3, signalReceived);
        }

        [Test]
        public void Test_Signal_Empty_Performance()
        {
            var signalHub = new SignalHub();

            var isSignalReceived = false;

            var stopwatch = Stopwatch.StartNew();
            signalHub.Subscribe<TestSignal>(this, () => isSignalReceived = true);
            for (var i = 0; i < 1000000; i++)
                signalHub.Dispatch(new TestSignal());
            Debug.Log($"Stopwatch finished in [<b>{stopwatch.Elapsed.TotalMilliseconds}</b> ms]");

            Assert.IsTrue(isSignalReceived);
        }

        [Test]
        public void Test_Signal_WithData()
        {
            var signalHub = new SignalHub();

            var isSignalReceived = false;

            signalHub.Subscribe(this, new TestSignalWithData(5), () => isSignalReceived = true);

            signalHub.Dispatch(new TestSignalWithData(1));
            Assert.IsFalse(isSignalReceived);

            signalHub.Dispatch(new TestSignalWithData(5));
            Assert.IsTrue(isSignalReceived);
        }

        [Test]
        public void Test_Signal_WithData_ReceiveData()
        {
            var signalHub = new SignalHub();

            var isSignalReceived = false;

            signalHub.Subscribe(this, new TestSignalWithData(5), (signal) => isSignalReceived = true);

            signalHub.Dispatch(new TestSignalWithData(1));
            Assert.IsFalse(isSignalReceived);

            signalHub.Dispatch(new TestSignalWithData(5));
            Assert.IsTrue(isSignalReceived);
        }

        [Test]
        public void Test_Signal_WithData_Performance()
        {
            var signalHub = new SignalHub();

            var isSignalReceived = false;

            var stopwatch = Stopwatch.StartNew();
            signalHub.Subscribe(this, new TestSignalWithData(5), () => isSignalReceived = true);

            for (var i = 0; i < 500000; i++)
                signalHub.Dispatch(new TestSignalWithData(1));

            for (var i = 0; i < 500000; i++)
                signalHub.Dispatch(new TestSignalWithData(5));

            Debug.Log($"Stopwatch finished in [<b>{stopwatch.Elapsed.TotalMilliseconds}</b> ms]");
        }

        [Test]
        public void Test_Signal_WithData_Generic()
        {
            var signalHub = new SignalHub();

            var isSignalReceived = false;
            signalHub.Subscribe<TestSignalWithData>(this, () => isSignalReceived = true);

            // since we didn't specify the data we're looking for -> any kind of signal will trigger the event
            signalHub.Dispatch(new TestSignalWithData(1));
            Assert.IsTrue(isSignalReceived);
        }

        [Test]
        public void Test_Signal_WithData_Conditional()
        {
            var signalHub = new SignalHub();

            var isSignalReceived = false;
            signalHub.Subscribe<TestSignalWithData>(this, signal => signal.testData == 5, () => isSignalReceived = true);

            signalHub.Dispatch(new TestSignalWithData(1));
            Assert.IsFalse(isSignalReceived);

            signalHub.Dispatch(new TestSignalWithData(5));
            Assert.IsTrue(isSignalReceived);
        }

        [Test]
        public void Test_Signal_WithData_Generic_ReceiveData()
        {
            var signalHub = new SignalHub();

            var isSignalReceived = false;
            signalHub.Subscribe<TestSignalWithData>(this, (signal) => isSignalReceived = true);

            // since we didn't specify the data we're looking for -> any kind of signal will trigger the event
            signalHub.Dispatch(new TestSignalWithData(1));
            Assert.IsTrue(isSignalReceived);
        }
    }
}