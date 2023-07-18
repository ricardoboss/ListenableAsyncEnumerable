namespace System.Collections.Generic;

public interface IRiver<T> : IAsyncEnumerable<T>
{
    /// <summary>
    /// Registers callbacks to this <see cref="River{T}"/> to get notified whenever data is available or an error occurs.
    /// </summary>
    /// <param name="onData">Invoked with the newly arrived data.</param>
    /// <param name="onError">Invoked with any exception thrown when moving to the next piece of data.</param>
    /// <param name="onDone">Invoked when the stream ends and no further data is expected.</param>
    /// <param name="cancelOnError">Whether to cancel the subscription when an error occurs.</param>
    /// <returns>A new <see cref="IRiverSubscription{T}"/> that can be used to control the subscription behaviour.</returns>
    public IRiverSubscription<T> Listen(Action<T>? onData = null, Action<Exception>? onError = null, Action? onDone = null, bool cancelOnError = true);
}