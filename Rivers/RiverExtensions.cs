namespace System.Collections.Generic;

public static class RiverExtensions
{
    public static IRiver<T> ToRiver<T>(this IAsyncEnumerable<T> enumerable)
    {
        if (enumerable is IRiver<T> river)
            return river;

        return new River<T>(enumerable.GetAsyncEnumerator());
    }
}