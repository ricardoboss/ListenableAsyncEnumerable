namespace System.Collections.Generic;

public static class ListenenableAsyncEnumerableExtensions
{
    public static IListenableAsyncEnumerable<T> ToListenable<T>(this IAsyncEnumerable<T> enumerable)
    {
        if (enumerable is IListenableAsyncEnumerable<T> listenableAsyncEnumerable)
            return listenableAsyncEnumerable;

        return new ListenableAsyncEnumerable<T>(enumerable.GetAsyncEnumerator());
    }
}