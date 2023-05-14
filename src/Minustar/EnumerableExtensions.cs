namespace Minustar;

/// <summary>
/// Provides extension methods for enumerable objects
/// </summary>
public static class EnumerableExtensions 
{
    /// <summary>
    /// Returns a value indicating whether the specified enumerable
    /// passes the specified predicate; or not.
    /// </summary>
    /// <param name="items">
    /// The <see cref="IEnumerable{T}"/> to check.
    /// </param>
    /// <param name="predicate">
    /// The predicate to test each element of the <see cref="IEnumerable{T}"/> on.
    /// </param>
    /// <param name="startIndex">
    /// The index of the first element in the <see cref="IEnumerable{T}"/> to check.
    /// </param>
    /// <param name="count">
    /// The number of elements in the <see cref="IEnumerable{T}"/> to check.
    /// </param>
    /// <typeparam name="T">
    /// The type of the elements in the <see cref="IEnumerable{T}"/>.
    /// </typeparam>
    /// <returns>
    /// <see langword="true"/> if the specified enumerable
    /// passes the specified predicate; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Check<T>(this IEnumerable<T> items, Func<T, bool> predicate, int startIndex, int count)
        => Check(items, predicate, startIndex, count, out int length) && count == length;

     /// <summary>
    /// Returns a value indicating whether the specified enumerable
    /// passes the specified predicate; or not.
    /// </summary>
    /// <param name="items">
    /// The <see cref="IEnumerable{T}"/> to check.
    /// </param>
    /// <param name="predicate">
    /// The predicate to test each element of the <see cref="IEnumerable{T}"/> on.
    /// </param>
    /// <param name="startIndex">
    /// The index of the first element in the <see cref="IEnumerable{T}"/> to check.
    /// </param>
    /// <param name="count">
    /// The number of elements in the <see cref="IEnumerable{T}"/> to check.
    /// </param>
    /// <param name="length">
    /// The number of elements in the <see cref="IEnumerable{T}"/> that passed the predicate.
    /// </param>
    /// <typeparam name="T">
    /// The type of the elements in the <see cref="IEnumerable{T}"/>.
    /// </typeparam>
    /// <returns>
    /// <see langword="true"/> if the specified enumerable
    /// passes the specified predicate; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Check<T>(this IEnumerable<T> items, Func<T,bool> predicate, int startIndex, int count, out int length) 
    {
        for (length = 0; length < count; length++)
        {
            if (!predicate(items.ElementAt(startIndex + length)))
                return length > 0;
        }

        return true;
    }
}
