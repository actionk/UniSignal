using System;

namespace Plugins.UniSignal
{
    public interface ISignal
    {
    }

    public interface ISignal<TKey> : ISignal, IEquatable<TKey>
    {
    }
}