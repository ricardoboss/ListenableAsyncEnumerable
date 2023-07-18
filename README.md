`Rivers` is a C# implementation of [Dart](https://dart.dev)s `Stream` infrastructure.

For now, only basic functionality is covered.

# IRiver\<T>

To obtain an `IRiver<T>`, call the `ToRiver` extension method on any `IAsyncEnumerable<T>`:

```csharp
IAsyncEnumerable<string> enumerable = GetMyAsyncEnumerable();
IRiver<string> river = enumerable.ToRiver();
```

# IRiverSubscription\<T>

Call the `Listen` method on an `IRiver<T>` to obtain an `IRiverSubscription<T>`:

```csharp
using IRiverSubscription<string> subscription = river.Listen(
    onData: value => HandleMyValue(value),
    onError: exception => HandleException(exception),
    onDone: () => HandleRiverDone()
);

await foreach (var value in river) {
    // Do something with the value
}

// Cancel the subscription if you don't need it anymore
// It will also get cancelled when Dispose is called
subscription.Cancel();
```

Calling `Listen` will _not_ enumerate the enumerable.
The callbacks passed to the `Listen` method only get invoked once the enumerable actually gets enumerated.

# Why 'River'?

"river" and "stream" are similar in their meaning, that's why.

I would have liked to reuse the naming used by Dart for this library, but .NET already provides a `Stream`, that is
exclusive for IO operations and memory management.

# Contributions

Contributions are welcome. Open an issue or a PR if you want.

# License

This project is licensed under the [MIT](./LICENSE) license.
