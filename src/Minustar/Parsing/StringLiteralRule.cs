namespace Minustar.Parsing;

/// <summary>
/// A rule that matches a <see cref="string"/> literal.
/// </summary>
public sealed class StringLiteralRule
    : Rule
{
    /// <summary>
    /// Gets the <see cref="string"/> literal to match.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets a value indicating whether the match
    /// should be case-insensitive; or not.
    /// </summary>
    public bool IgnoreCase { get; }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="StringLiteralRule"/> class.
    /// </summary>
    /// <param name="value">
    /// The <see cref="string"/> literal to match.
    /// </param>
    /// <param name="ignoreCase">
    /// <see langword="true"/> if the match should be case-insensitive;
    /// otherwise <see langword="false"/>.
    /// </param>
    public StringLiteralRule(string value, bool ignoreCase)
    {
        Value = value;
        IgnoreCase = ignoreCase;
    }

    /// <inheritdoc/>
    public override bool TryMatch(string str, int index, out int length)
    {
        var comparer = IgnoreCase
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        if (str.IndexOf(Value, index, comparer) == index)
        {
            length = Value.Length;
            return true;
        }

        length = 0;
        return false;
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"\'{Value.EscapeStringLiteral()}\'";
}
