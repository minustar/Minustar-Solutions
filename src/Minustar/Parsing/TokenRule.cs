namespace Minustar.Parsing;

/// <summary>
/// A rule than be match a token.s
/// </summary>
public abstract class TokenRule
    : IRule
{
    /// <summary>
    /// Gets the name of the token that this rule matches.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the <see cref="Rule"/> that matches the token.
    /// </summary>
    public Rule Rule { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenRule"/> class.
    /// </summary>
    /// <param name="name">
    /// The name of the token that this rule matches.
    /// </param>
    /// <param name="rule">
    /// The <see cref="Rule"/> that matches the token.
    /// </param>
    public TokenRule(string name, Rule rule)
    {
        Name = name;
        Rule = rule;
    }

    /// <inheritdoc/>
    public abstract bool TryMatch(string str, int index, out int length);
}
