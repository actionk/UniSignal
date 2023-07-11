using UnityEngine;

namespace Plugins.UniSignal.Core
{
    public class AsyncSignalManagerUpdater : MonoBehaviour
    {
        private AsyncSignalManager m_asyncSignalManager;

        private void Awake()
        {
            m_asyncSignalManager = AsyncSignalManager.Instance;
        }

        private void Update()
        {
            m_asyncSignalManager.Update();
        }
    }
}