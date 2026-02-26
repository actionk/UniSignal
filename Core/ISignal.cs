using System;

namespace UniSignal
{
    public interface ISignal
    {
    }

    public interface ISignal<TKey> : ISignal, IEquatable<TKey>
    {
    }
}