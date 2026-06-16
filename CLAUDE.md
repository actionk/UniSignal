# UniSignal

## Purpose

AI guidance for this Unity package. Keep package work small and compatible with Unity 2021.3+.

UniSignal is a lightweight, type-safe signal library for same-process Unity events. It is for decoupling systems that already share one runtime, not for persistence, networking, authority, or cross-thread messaging.

Read `README.md` for public API examples before changing behavior. Source of truth is `Core/SignalHub.cs`, `Core/ISignal.cs`, and `Subscriptions/`.

## Use UniSignal When

* One runtime system needs to notify another without a direct reference.
* Client presentation code needs transient events from simulation/view-adapter code.
* A scene, feature, or subsystem owns a `SignalHub` and can clean it up with that owner.
* Signals are fire-and-forget notifications, not durable state.

## Do Not Use It For

* Gameplay authority, save data, replicated network state, or command validation.
* Long-lived global state. Create and inject/own `SignalHub` instances explicitly.
* Event streams that need ordering guarantees across threads or frames.
* Per-frame polling, data queries, or replacing plain method calls inside one class.

## Core Rules

* Signals are `struct` payloads implementing `ISignal`.
* Use `ISignal<T>` only when subscribers need value-specific matching via `Equals`/`GetHashCode`.
* Keep payloads small. Prefer ids and snapshots over mutable objects.
* Pass a listener object when subscribing, usually `this`.
* Unsubscribe by listener during teardown: `signalHub.Unsubscribe(this)`.
* Avoid listenerless subscriptions unless they are intentionally permanent.
* Subscribe once during setup, not repeatedly in `Update`.
* Dispatch is reentrant-safe: subscribe/unsubscribe during callbacks is queued until current dispatch completes.
* `SignalHub` is not thread-safe. Use it from Unity main thread unless caller provides external synchronization.

## Unity Usage

* Own hubs from composition roots, scene services, feature services, or MonoBehaviour scopes.
* With DI, register `SignalHub` with the lifetime that matches the subscribers.
* In MonoBehaviours, subscribe in `OnEnable`/setup and unsubscribe in `OnDisable`/`OnDestroy`.
* In ECS integrations, use signals for presentation side effects only. Authoritative state still belongs in ECS/network data.

## Change Rules

* Preserve allocation-free dispatch after warmup.
* Preserve subscription pooling.
* Preserve safe subscribe/unsubscribe during dispatch.
* Keep API names simple; do not add aliases or wrappers without a real use case.
* Add or update Unity Test Runner tests in `Tests/` for behavior changes.

## Release Rules

* Bump `package.json` version for releases.
* Keep README install snippets on the current release tag.
* Tag releases with the package version, for example `1.0.1`.
