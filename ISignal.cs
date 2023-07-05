using System;

namespace Plugins.Polymorphex.Packages.UniSignal
{
    public interface ISignal
    {
    }

    public interface ISignal<TKey> : ISignal, IEquatable<TKey>
    {
    }
}