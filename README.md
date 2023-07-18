`ListenableAsyncEnumerable` is a C# implementation of [Dart](https://dart.dev)s `Stream` infrastructure.

For now, only basic functionality is covered.

# IListenableAsyncEnumerable\<T>

To obtain an `IListenableAsyncEnumerable<T>`, call the `ToListenable` extension method on any `IAsyncEnumerable<T>`:

```csharp
IAsyncEnumerable<string> enumerable = GetMyAsyncEnumerable();
IListenableAsyncEnumerable<string> listenable = enumerable.ToListenable();
```

# IAsyncEnumerableSubscription\<T>

Call the `Listen` method on an `IListenableAsyncEnumerable<T>` to obtain an `IAsyncEnumerableSubscription<T>`:

```csharp
using IAsyncEnumerableSubscription<string> subscription = listenable.Listen(
    onData: value => HandleMyValue(value),
    onError: exception => HandleException(exception),
    onDone: () => HandleRiverDone()
);

await foreach (var value in listenable) {
    // Do something with the value
}

// Cancel the subscription if you don't need it anymore
// It will also get cancelled when Dispose is called
subscription.Cancel();
```

Calling `Listen` will _not_ enumerate the enumerable.
The callbacks passed to the `Listen` method only get invoked once the enumerable actually gets enumerated.

# Contributions

Contributions are welcome. Open an issue or a PR if you want.

# License

This project is licensed under the [MIT](./LICENSE) license.
