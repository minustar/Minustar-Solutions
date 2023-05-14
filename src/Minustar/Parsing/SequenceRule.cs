namespace Minustar.Parsing;

/// <summary>
/// A rule that matches a sequence of rules.
/// </summary>
public sealed class SequenceRule
    : Rule
{
    /// <summary>
    /// Gets the sequence of rules to match.
    /// </summary>
    public IEnumerable<Rule> Rules { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SequenceRule"/> class.
    /// </summary>
    /// <param name="rules">
    /// The sequence of rules to match.
    /// </param>
    public SequenceRule(IEnumerable<Rule> rules)
    {
        Rules = rules;
    }

    /// <inheritdoc/>
    public override bool TryMatch(string str, int index, out int length)
    {
        length = 0;
        foreach (var rule in Rules)
        {
            if (!rule.TryMatch(str, index + length, out var ruleLength))
                return false;

            length += ruleLength;
        }

        return true;
    }

    /// <inheritdoc/>
    public override string ToString()
        => string.Join(" ", Rules);
}
