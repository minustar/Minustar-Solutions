namespace Minustar.Parsing;

/// <summary>
/// A rule that matches characters based on groups.
/// </summary>
public sealed class CharGroupRule
    : Rule
{
    /// <summary>
    /// Gets the groups of characters that this rule matches.
    /// </summary>
    public IEnumerable<CharGroup> Groups { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CharGroupRule"/> class.
    /// </summary>
    /// <param name="groups">
    /// The groups of characters that this rule matches.
    /// </param>
    public CharGroupRule(IEnumerable<CharGroup> groups)
    {
        Groups = groups;
    }

    /// <inheritdoc/>
    public override bool TryMatch(string str, int index, out int length)
    {
        foreach (var group in Groups)
            if (group.TryMatch(str, index, out length))
                return true;

        length = 0;
        return false;
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"[{string.Join(null, Groups)}]";
}
