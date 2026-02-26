using System.Collections.Generic;

namespace UniSignal.Utils
{
    /// <summary>
    /// Simple stack-based object pool for reducing GC allocations.
    /// </summary>
    internal class ObjectPool<T> where T : class, new()
    {
        private readonly Stack<T> m_pool;
        private readonly int m_maxSize;

        public ObjectPool(int initialCapacity = 8, int maxSize = 64)
        {
            m_pool = new Stack<T>(initialCapacity);
            m_maxSize = maxSize;
        }

        public T Get()
        {
            return m_pool.Count > 0 ? m_pool.Pop() : new T();
        }

        public void Return(T item)
        {
            if (item != null && m_pool.Count < m_maxSize)
                m_pool.Push(item);
        }

        public void Clear()
        {
            m_pool.Clear();
        }

        public int Count => m_pool.Count;
    }
}
