namespace System.Collections.Generic;

public class AsyncEnumerableSubscription<T> : IAsyncEnumerableSubscription<T>
{
    private readonly ListenableAsyncEnumerable<T> source;
    private readonly bool cancelOnError;

    public AsyncEnumerableSubscription(ListenableAsyncEnumerable<T> source, Action<T> onData, Action<Exception> onError, Action onDone, bool cancelOnError)
    {
        this.source = source;
        this.cancelOnError = cancelOnError;

        OnData = onData;
        OnError = onError;
        OnDone = onDone;
    }

    /// <inheritdoc />
    public Action<T> OnData { get; set; }

    /// <inheritdoc />
    public Action<Exception> OnError { get; set; }

    /// <inheritdoc />
    public Action OnDone { get; set; }

    /// <inheritdoc />
    public Task<TResult?> AsTask<TResult>(TResult? doneValue = default, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<TResult?>();

        OnDone = () => tcs.TrySetResult(doneValue);
        OnError = e => tcs.TrySetException(e);

        cancellationToken.Register(() =>
        {
            tcs.TrySetCanceled(cancellationToken);

            Cancel();
        });

        return tcs.Task;
    }

    /// <inheritdoc />
    public Task AsTask(CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource();

        OnDone = () => tcs.TrySetResult();
        OnError = e => tcs.TrySetException(e);

        cancellationToken.Register(() =>
        {
            tcs.TrySetCanceled(cancellationToken);
            
            Cancel();
        });

        return tcs.Task;
    }

    /// <inheritdoc />
    public void Cancel() => source.RemoveSubscription(this);

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        Cancel();
    }

    internal void NotifyMovedNext(bool success)
    {
        if (success)
            OnData(source.GetAsyncEnumerator().Current);
        else
            OnDone();
    }

    internal void NotifyException(Exception e)
    {
        OnError(e);

        if (cancelOnError)
            Cancel();
    }
}