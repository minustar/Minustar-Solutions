namespace Minustar.Parsing;

/// <summary>
/// A token emitted by a tokenizer.
/// </summary>
public record Token
{
    /// <summary>
    /// Gets the line number of the token.
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Gets the column number of the token.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Gets the type of the token.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the value of the token.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets a value indicating whether the token is skippable;
    /// or not.
    /// </summary>
    public bool Skippable { get; }

    /// <summary>
    /// Gets the type of the previous token,
    /// if applicable.
    /// </summary>
    public string? PreviousTokenType { get; init; }

    /// <summary>
    /// Initialises a new instance of the <see cref="Token"/> class.
    /// </summary>
    /// <param name="line">
    /// The line number of the token.
    /// </param>
    /// <param name="column">
    /// The column number of the token.
    /// </param>
    /// <param name="type">
    /// The type of the token.
    /// </param>
    /// <param name="value">
    /// The value of the token.
    /// </param>
    /// <param name="skippable">
    /// <see langword="true"/> if the token is skippable;
    /// otherwise, <see langword="false"/>.
    /// </param>
    public Token(int line, int column, string type, string value, bool skippable = false)
    {
        Line = line;
        Column = column;
        Type = type;
        Value = value;
        Skippable = skippable;
    }

    /// <summary>
    /// Gets an invalid token singleton.
    /// </summary>
    public static Token InvalidToken { get; } = new(-1, -1, string.Empty, string.Empty);

    /// <summary>
    /// Deconstructs the token into its components.
    /// </summary>
    /// <param name="line">
    /// The line number of the token.
    /// </param>
    /// <param name="column">
    /// The column number of the token.
    /// </param>
    /// <param name="type">
    /// The type of the token.
    /// </param>
    /// <param name="value">
    /// The value of the token.
    /// </param>
    /// <param name="skippable">
    /// <see langword="true"/> if the token is skippable;
    /// otherwise, <see langword="false"/>.
    /// </param>
    public void Deconstruct(out int line, out int column, out string type, out string value, out bool skippable)
    {
        line = Line;
        column = Column;
        type = Type;
        value = Value;
        skippable = Skippable;
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"({Line}, {Column}) {Type} \"{Value.EscapeStringLiteral()}\"";
}
