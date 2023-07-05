# UniSignal [![Github license](https://img.shields.io/github/license/codewriter-packages/Morpeh.Events.svg?style=flat-square)](#) [![Unity 2020](https://img.shields.io/badge/Unity-2020+-2296F3.svg?style=flat-square)](#) ![GitHub package.json version](https://img.shields.io/github/package-json/v/actionk/UniSignal?style=flat-square)

UniSignal is a flexible and performance-focused C# signal management library, tailored for Unity. It supports subscribing and dispatching signals based on class type, specific data, or custom predicates.

## Table of Contents

- [Getting Started](#getting-started)
- [Usage](#usage)
  - [SignalHub](#signalhub)
  - [Subscribing to Empty Signals](#subscribing-to-empty-signals)
  - [Subscribing to Signals with Specific Content](#subscribing-to-signals-with-specific-content)
  - [Subscribing to Signals with a Predicate](#subscribing-to-signals-with-a-predicate)
- [Unsubscribing](#unsubscribing)
  - [Unsubscribing from Specific Subscription](#unsubscribing-from-specific-subscription)
  - [Unsubscribing from All Subscriptions of This Listener](#unsubscribing-from-all-subscriptions-of-this-listener)
- [Tests and Examples](#tests-and-examples)
- [Contributing](#contributing)
- [License](#license)

## Getting Started

Open the Package Manager in Unity, click on "+" button, choose "Add package from git URL..." and paste:
```
https://github.com/actionk/UniSignal.git#0.1
```

## Usage

### SignalHub

In order to start using UniSignal, you need an instance of SignalHub.

```csharp
var signalHub = new SignalHub();
```

### Subscribing to Empty Signals

One can subscribe to signals by their type and the callback will be called for any signal of this type.

```csharp
signalHub.Subscribe<TestSignal>(this, () => Debug.Log("Received signal"));
```

When subscribing to a signal, you have the option to pass an object as the listener ("this" in that case). This object represents the owner of the subscription. It's useful for when you want to unsubscribe all signals tied to a specific object, for example when an object is destroyed or no longer needs to listen for signals.

### Subscribing to Signals with Specific Content

In this case, one should inherit from `ISignal<>`, which implements `IEquatable<T>` to check if that's exactly the signal we're waiting for.

```csharp
signalHub.Subscribe(this, new TestSignalWithData(5), () => Debug.Log("Received specific signal"));
```

### Subscribing to Signals with a Predicate

In this case, the callback will only be called for signals of the specified type that are valid according to the predicate.

```csharp
signalHub.Subscribe<TestSignalWithData>(this, signal => signal.testData == 5, () => Debug.Log("Received signal with predicate"));
```

## Unsubscribing

### Unsubscribing from Specific Subscription

```csharp
var subscription = signalHub.Subscribe<TestSignal>(this, () => Debug.Log("Received signal"));
signalHub.Unsubscribe(subscription);
```

### Unsubscribing from All Subscriptions of This Listener

```csharp
signalHub.Subscribe<TestSignal>(this, () => Debug.Log("Received signal"));
signalHub.Unsubscribe(this);
```

## Tests and Examples

Please refer to the `SignalsTest.cs` file for examples and test cases.

## Contributing

Any contributions you make are greatly appreciated.

## License

Distributed under the MIT License. See `LICENSE` for more information.
