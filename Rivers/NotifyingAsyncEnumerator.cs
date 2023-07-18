namespace Rivers;

public class NotifyingAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IAsyncEnumerator<T> source;

    public NotifyingAsyncEnumerator(IAsyncEnumerator<T> source)
    {
        this.source = source;
    }

    public event MovedNextEventArgs? OnMovedNext;
    public event ExceptionThrownEventArgs? OnExceptionThrown;

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await source.DisposeAsync();
    }

    /// <inheritdoc />
    public async ValueTask<bool> MoveNextAsync()
    {
        try
        {
            var result = await source.MoveNextAsync();

            OnMovedNext?.Invoke(result);

            return result;
        }
        catch (Exception e)
        {
            OnExceptionThrown?.Invoke(e);

            throw;
        }
    }

    /// <inheritdoc />
    public T Current => source.Current;
}