namespace Minustar.Parsing;

/// <summary>
/// A rule for parsing a token.
/// </summary>
public abstract class Rule
{
    /// <inheritdoc/>
    public abstract bool TryMatch(string str, int index, out int length);

    /// <inheritdoc/>
    public abstract override string ToString();

    /// <summary>
    /// Creates a new rule that quantifies another rule.
    /// </summary>
    /// <param name="rule">
    /// The rule to quantify.
    /// </param>
    /// <param name="min">
    /// The minimum number of times the rule must match.
    /// </param>
    /// <param name="max">
    /// The maximum number of times the rule can match.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="Rule"/>.
    /// </returns>
    public static Rule Quantify(Rule rule, int min, int? max)
        =>  new QuantifiedRule(rule, min, max);

    /// <summary>
    /// Creates a new rule that can match zero or one times.
    /// </summary>
    /// <param name="rule">
    /// The rule to match.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="Rule"/>.
    /// </returns>
  public static Rule Optional(Rule rule)
        => Quantify(rule, 0, 1);

    /// <summary>
    /// Creates a new rule that can match once or more times.
    /// </summary>
    /// <param name="rule">
    /// The rule to match.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="Rule"/>.
    /// </returns>
    public static Rule OneOrMany(Rule rule)
        => Quantify(rule, 1, null);

    /// <summary>
    /// Creates a new rule that can match zero or more times.
    /// </summary>
    /// <param name="rule">
    /// The rule to match.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="Rule"/>.
    /// </returns>
    public static Rule Zero0rMany(Rule rule)
        => Quantify(rule, 0, null);

    /// <summary>
    /// Creates a new rule that matches character groups.
    /// </summary>
    /// <param name="groups">
    /// The groups to match.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="Rule"/>.
    /// </returns>
    public static Rule Group(params CharGroup[] groups)
        => new CharGroupRule(groups);

    /// <summary>
    /// Creates a new rule that matches a <see cref="string"/> literal.
    /// </summary>
    /// <param name="literal">
    /// The literal to match.
    /// </param>
    /// <param name="ignoreCase">
    /// <see langword="true"/> to ignore case;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="Rule"/>.
    /// </returns>
    public static Rule Literal(string literal, bool ignoreCase = false)
        => new StringLiteralRule(literal, ignoreCase);

    /// <summary>
    /// Creates a new rule that matches a sequence of rules.
    /// </summary>
    /// <param name="rules">
    /// The sequence of rules to match.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="Rule"/>.
    /// </returns>
    public static Rule Sequence(params Rule[] rules)
        => new SequenceRule(rules);
}
