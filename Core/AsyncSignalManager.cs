using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Plugins.UniSignal.Core
{
    public class AsyncSignalManager
    {
        // Public property to access the singleton instance
        public static AsyncSignalManager Instance => INSTANCE.Value;

        private static readonly Lazy<AsyncSignalManager> INSTANCE = new(() => new AsyncSignalManager(), LazyThreadSafetyMode.ExecutionAndPublication);

        private interface IAsyncSignalQueueEntry
        {
            void Dispatch();
        }

        private struct AsyncSignalQueueEntry<T> : IAsyncSignalQueueEntry where T : struct, ISignal
        {
            public SignalHub signalHub;
            public T signal;

            public void Dispatch()
            {
                signalHub.Dispatch(signal);
            }
        }

        private readonly List<IAsyncSignalQueueEntry> m_queuedSignals = new();

        public AsyncSignalManager()
        {
        }

        /// <summary>
        /// Call this method from any of your schedulers or use <see cref="AsyncSignalManagerUpdater"/>
        /// </summary>
        public void Update()
        {
            var queuedSignals = FlushSignals();
            foreach (var signal in queuedSignals)
                signal.Dispatch();
        }

        internal void AddSignal<T>(SignalHub signalHub, T signal) where T : struct, ISignal
        {
            lock (m_queuedSignals)
            {
                m_queuedSignals.Add(new AsyncSignalQueueEntry<T>
                {
                    signalHub = signalHub,
                    signal = signal
                });
            }
        }

        private IEnumerable<IAsyncSignalQueueEntry> FlushSignals()
        {
            lock (m_queuedSignals)
            {
                var signals = m_queuedSignals.ToList();
                m_queuedSignals.Clear();
                return signals;
            }
        }
    }
}