namespace Minustar;

/// <summary>
/// Provides extension methods for the <see cref="string"/> class.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Returns the UTF-32 code point at the specified index in the string.
    /// </summary>
    /// <param name="str">
    /// The string to get the code point from.
    /// </param>
    /// <param name="index">
    /// The index of the code point.
    /// </param>
    /// <param name="codePointLength">
    /// The length of the code point in UTF-16 code units.
    /// i.e. 1 for BMP code points, 2 for supplementary code points
    /// that are represented as surrogate pairs.
    /// </param>
    /// <returns>
    /// The UTF-32 code point at the specified index in the string.
    /// </returns>
    public static int GetCodePoint(this string str, int index, out int codePointLength)
    {
        if (str is null)
            throw new ArgumentNullException(nameof(str));
        if (index < 0 || index >= str.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        var codePoint = char.ConvertToUtf32(str, index);
        codePointLength = char.IsSurrogatePair(str, index) ? 2 : 1;
        return codePoint;
    }

    /// <summary>
    /// Returns a character at the specified index in the string
    /// is a valid hexadecimal digit; or not.
    /// </summary>
    /// <param name="str">
    /// The string to check.
    /// </param>
    /// <param name="index">
    /// The index of the character to check.
    /// </param>
    /// <param name="codePoint">
    /// The UTF-32 code point at the specified index in the string,
    /// if the character is a valid hexadecimal digit.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the character at the specified index
    /// in the string is a valid hexadecimal digit;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsHexDigit(this string str, int index, out int codePoint)
    {
        if (str is null)
            throw new ArgumentNullException(nameof(str));
        if (index < 0 || index >= str.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        codePoint = str.GetCodePoint(index, out _);
        return (codePoint >= 'A' && codePoint <= 'F')
            || (codePoint >= 'a' && codePoint <= 'f')
            || (codePoint >= '0' && codePoint <= '9');
    }

    /// <summary>
    /// Returns a character at the specified index in the string
    /// is a valid hexadecimal digit; or not.
    /// </summary>
    /// <param name="str">
    /// The string to check.
    /// </param>
    /// <param name="index">
    /// The index of the character to check.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the character at the specified index
    /// in the string is a valid hexadecimal digit;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsHexDigit(this string str, int index)
        => IsHexDigit(str, index, out _);
}
