namespace System.Collections.Generic;

/// <summary>
/// Extending <see cref="IAsyncEnumerable{T}"/>, a <see cref="River{T}"/> adds the ability to add
/// <see cref="RiverSubscription{T}"/>s to any given <see cref="IAsyncEnumerator{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the elements in the collection</typeparam>
public class River<T> : IRiver<T>
{
    private readonly NotifyingAsyncEnumerator<T> enumerator;

    public River(IAsyncEnumerator<T> enumerator)
    {
        if (enumerator is NotifyingAsyncEnumerator<T> notifyingAsyncEnumerator)
            this.enumerator = notifyingAsyncEnumerator;
        else
            this.enumerator = new(enumerator);
    }

    /// <inheritdoc />
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new()) => enumerator;

    /// <inheritdoc />
    public IRiverSubscription<T> Listen(Action<T>? onData = null, Action<Exception>? onError = null, Action? onDone = null, bool cancelOnError = true)
    {
        var realOnData = onData ?? (_ => { });
        var realOnError = onError ?? (_ => { });
        var realOnDone = onDone ?? (() => { });

        var subscription = new RiverSubscription<T>(this, realOnData, realOnError, realOnDone, cancelOnError);

        enumerator.OnMovedNext += subscription.NotifyMovedNext;
        enumerator.OnExceptionThrown += subscription.NotifyException;

        return subscription;
    }

    internal void RemoveSubscription(RiverSubscription<T> subscription)
    {
        enumerator.OnMovedNext -= subscription.NotifyMovedNext;
        enumerator.OnExceptionThrown -= subscription.NotifyException;
    }
}