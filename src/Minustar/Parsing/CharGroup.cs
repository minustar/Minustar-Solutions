namespace Minustar.Parsing;

/// <summary>
/// A group of characters that can be matched.
/// </summary>
public abstract class CharGroup
    : IRule
{
    /// <inheritdoc/>
    public bool TryMatch(string str, int index, out int length)
    {
        var codePoint = str.GetCodePoint(index, out length);
        return TryMatch(codePoint);
    }

    /// <summary>
    /// Tries to match the specified character.
    /// </summary>
    /// <param name="character">
    /// The code point of the character to match.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the character was matched;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public abstract bool TryMatch(int character);

    /// <summary>
    /// Creates a new character group that matches the specified character.
    /// </summary>
    /// <param name="ch">
    /// The code point of the character to match.
    /// </param>
    /// <returns>
    /// A new <see cref="CharGroup"/> instance.
    /// </returns>
    public static CharGroup Character(int ch)
        => new CharacterGroup(ch);

    /// <summary>
    /// Creates a new character group that matches any character in the
    /// specified range.
    /// </summary>
    /// <param name="start">
    /// The code point of the first character in the range.
    /// </param>
    /// <param name="end">
    /// The code point of the last character in the range.
    /// </param>
    /// <returns>
    /// A new <see cref="CharGroup"/> instance.
    /// </returns>
    public static CharGroup Range(int start, int end)
        => new CodePointRangeCharGroup(start, end);

    /// <summary>
    /// Creates a new character group that matches any character in the
    /// specified Unicode categories.
    /// </summary>
    /// <param name="selector">
    /// The selector for the Unicode categories to match.
    /// </param>
    /// <returns>
    /// A new <see cref="CharGroup"/> instance.
    /// </returns>
    public static CharGroup Category(string selector)
        => new UnicodeCategoryCharGroup(selector);

    /// <summary>
    /// Escapes the specified character for use in a character group.
    /// </summary>
    /// <param name="ch">
    /// The code point of the character to escape.
    /// </param>
    /// <returns>
    /// The escaped character.
    /// </returns>
    protected static string EscapeCharacter(int ch)
    {
        if (ch == '-')
            return "\\-";
        else if (ch == ']')
            return "\\]";

        var str = char.ConvertFromUtf32(ch);
        return str.EscapeStringLiteral();
    }
}
