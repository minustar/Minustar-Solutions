namespace Minustar.Parsing;

/// <summary>
/// A character group that matches a single character.
/// </summary>
public sealed class CharacterGroup
    : CharGroup
{
    /// <summary>
    /// Gets the code point of the character that this group matches.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterGroup"/> class.
    /// </summary>
    /// <param name="value">
    /// The code point of the character that this group matches.
    /// </param>
    public CharacterGroup(int value)
    {
        if (value < 0 || value > 0x10FFFF)
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }

    /// <inheritdoc/>
    public override bool TryMatch(int codePoint)
    {
        var source = char.ConvertFromUtf32(codePoint);
        var target = char.ConvertFromUtf32(Value);

        source.Normalize(NormalizationForm.FormC);
        target.Normalize(NormalizationForm.FormC);

        return source == target;
    }

    /// <inheritdoc/>
    public override string ToString()
        => EscapeCharacter(Value);
}
