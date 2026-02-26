# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

UniSignal is a lightweight, zero-allocation signal (event) library for Unity. It provides type-safe, struct-based signals with no strings, no boxing, and no GC in steady state through object pooling.

**Package:** `com.actionik.polymorphex.unisignal` v1.0.0
**Unity:** 2021.3+
**Dependencies:** None (pure C#)

## Running Tests

Tests run via Unity's Test Runner: **Window → General → Test Runner**. There is no CLI test command — this is a Unity UPM package, not a standalone project.

## Architecture

### Three-Layer Design

**Core** (`Core/`) — `ISignal` marker interface, `ISignal<T>` for value-matchable signals (requires `IEquatable<T>`), and `SignalHub` as the central dispatcher.

**Subscriptions** (`Subscriptions/`) — Six concrete subscription types covering combinations of: anonymous (type-only) vs specific (value-matched), with/without data callback, with/without predicate filter. All inherit from `SignalSubscription<T>`. `SignalSubscriptionStorage<T>` manages per-signal-type subscription lists and dispatch.

**Utils** (`Utils/`) — `ObjectPool<T>` (stack-based, max 64) and `MultiValueDictionaryList<TKey, TValue>` (dictionary with List values per key).

### Key Dispatch Flow

1. `SignalHub` holds `Dictionary<Type, ISignalSubscriptionStorage>` mapping signal types to their storage
2. On `Subscribe`, the hub gets/creates a `SignalSubscriptionStorage<T>`, pulls a subscription from the static pool (`SignalHub.SubscriptionPools<T>`), and registers it
3. On `Dispatch`, the storage iterates anonymous subscriptions first, then value-specific subscriptions
4. Reentrancy is handled via `m_dispatchDepth` counter — mutations during dispatch are queued in `m_delayedActions` and applied when depth returns to 0
5. `SignalHub` also tracks `MultiValueDictionaryList<object, ISignalSubscription>` for bulk unsubscribe by listener object

### Subscription Types

| Class | Callback | Filtering |
|---|---|---|
| `SignalSubscriptionAnonymous<T>` | `Action` | Type-only |
| `SignalSubscriptionAnonymousWithData<T>` | `Action<T>` | Type-only |
| `SignalSubscriptionAnonymousConditional<T>` | `Action` | Predicate |
| `SignalSubscriptionAnonymousConditionalWithData<T>` | `Action<T>` | Predicate |
| `SignalSubscriptionSpecific<T>` | `Action` | Value equality |
| `SignalSubscriptionSpecificWithData<T>` | `Action<T>` | Value equality |

## Code Conventions

- **Namespaces:** `UniSignal`, `UniSignal.Subscriptions`, `UniSignal.Utils`
- **Private fields:** `m_camelCase` prefix
- **Signal naming:** `<Action>Signal` (e.g., `PlayerDiedSignal`, `ItemPickedUpSignal`)
- **No LINQ** — all iteration uses indexed for-loops to avoid enumerator allocations
- **No singletons** — multiple `SignalHub` instances are supported by design
- **Signals are structs** implementing `ISignal` (or `ISignal<T>` for value matching)

## Performance Constraints

Any changes must preserve zero-GC in steady state. Avoid:
- LINQ or foreach (enumerator allocations)
- Closures that capture variables (delegate allocations)
- Boxing value types
- Creating new objects in hot paths — use the existing `ObjectPool<T>` pattern
