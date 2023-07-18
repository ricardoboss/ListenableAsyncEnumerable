namespace System.Collections.Generic;

public interface IAsyncEnumerableSubscription<T> : IDisposable
{
    public Action<T> OnData { get; set; }

    public Action<Exception> OnError { get; set; }

    public Action OnDone { get; set; }

    /// <summary>
    /// Cancels this subscription and detaches the listeners from the subscribed <see cref="IListenableAsyncEnumerable{T}"/>. Does not
    /// invoke the <see cref="OnDone"/> callback.
    /// </summary>
    void Cancel();

    /// <summary>
    /// Returns a task that handles the <see cref="OnDone"/> and <see cref="OnError"/> callbacks.
    ///
    /// This method overwrite the existing <see cref="OnDone"/> and <see cref="OnError"/> callbacks with new ones that
    /// complete the returned task.
    ///
    /// In case of an error, the subscription will automatically cancel (even is it was listening with
    /// `cancelOnError` set to `false`).
    ///
    /// In case of a `done` event the future completes with the given <see cref="doneValue"/>.
    /// </summary>
    /// <param name="doneValue">The value returned when the river ends.</param>
    /// <param name="cancellationToken">A cancellation token passed to the returned task.</param>
    /// <typeparam name="TResult">The type of the result to return</typeparam>
    /// <returns>A nullable value of type <see cref="TResult"/>.</returns>
    public Task<TResult?> AsTask<TResult>(TResult? doneValue = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a task that handles the <see cref="OnDone"/> and <see cref="OnError"/> callbacks.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token passed to the returned task.</param>
    /// <returns>A task that completes when the <see cref="ListenableAsyncEnumerable{T}"/> does or throws then the <see cref="OnError"/>
    /// callback gets invoked.</returns>
    public Task AsTask(CancellationToken cancellationToken = default);
}