namespace Minustar.Parsing;

/// <summary>
/// A rule that matches a specified number of times.
/// </summary>
public sealed class QuantifiedRule
    : Rule
{
    /// <summary>
    /// Gets the rule that is quantified.
    /// </summary>
    public Rule Rule { get; }

    /// <summary>
    /// Gets the minimum number of times that the rule must be matched.
    /// </summary>
    public int Minimum { get; }

    /// <summary>
    /// Gets the maximum number of times that the rule can be matched;
    /// or if <see langword="null"/>, the rule can be matched any number
    /// of times.
    /// </summary>
    /// <value></value>
    public int? Maximum { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuantifiedRule"/> class.
    /// </summary>
    /// <param name="rule">
    /// The rule that is quantified.
    /// </param>
    /// <param name="min">
    /// The minimum number of times that the rule must be matched.
    /// </param>
    /// <param name="max">
    /// The maximum number of times that the rule can be matched;
    /// or if <see langword="null"/>, the rule can be matched any number
    /// of times.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="min"/> is less than zero.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="max"/> is less than <paramref name="min"/>.
    /// </exception>
    public QuantifiedRule(Rule rule, int min, int? max)
    {
        if (min < 0)
            throw new ArgumentOutOfRangeException(nameof(min));
        if (max.HasValue && max.Value < min)
            throw new ArgumentOutOfRangeException(nameof(max));

        Rule = rule;
        Minimum = min;
        Maximum = max;
    }

    /// <inheritdoc/>
    public override bool TryMatch(string str, int index, out int length)
    {
        var count = 0;
        var i = index;

        while (count < Minimum)
        {
            if (!Rule.TryMatch(str, i, out var len))
            {
                length = 0;
                return false;
            }

            i += len;
            count++;
        }

        while (!Maximum.HasValue || count < Maximum.Value)
        {
            if (!Rule.TryMatch(str, i, out var len))
                break;

            i += len;
            count++;
        }

        length = i - index;
        return true;
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"{Rule}{GetQuantifier()}";

    private string GetQuantifier()
    {
        var (n, m) = (Minimum, Maximum);

        return (n, m) switch
        {
            (0, 1) => "?",
            (1, 1) => "",
            (0, null) => "*",
            (1, null) => "+",
            (0, > 1) => $"{{,{m}}}",
            ( > 1, null) => $"{{{n},}}",
            _ when n == m => $"{{{n}}}",
            _ => $"{{{n},{m}}}"
        };
    }
}
