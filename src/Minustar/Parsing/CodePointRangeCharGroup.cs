namespace Minustar.Parsing;

/// <summary>
/// A character group that matches a single character
/// within a range of code points.
/// </summary>
public sealed class CodePointRangeCharGroup
    : CharGroup 
{
    /// <summary>
    /// Gets the code point of the first character in the range.
    /// </summary>
    public int Start { get; }

    /// <summary>
    /// Gets the code point of the last character in the range.
    /// </summary>
    public int End { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodePointRangeCharGroup"/> class.
    /// </summary>
    /// <param name="start">
    /// The code point of the first character in the range.
    /// </param>
    /// <param name="end">
    /// The code point of the last character in the range.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="start"/> or <paramref name="end"/> is less than
    /// U+0000 or greater than U+10FFFF.
    /// </exception>
    public CodePointRangeCharGroup(int start, int end)
    {
        if (start < 0 || start > 0x10FFFF)
            throw new ArgumentOutOfRangeException(nameof(start));
        if (end < 0 || end > 0x10FFFF)
            throw new ArgumentOutOfRangeException(nameof(end));

        Start = start;
        End = end;
    }

    /// <inheritdoc/>
    public override bool TryMatch(int character)
        => character >= Start
        && character <= End;
    
    /// <inheritdoc/>
    public override string ToString()
        => $"{EscapeCharacter(Start)}-{EscapeCharacter(End)}";
}
