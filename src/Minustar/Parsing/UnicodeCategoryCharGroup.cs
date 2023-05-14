namespace Minustar.Parsing;

/// <summary>
/// A rule that matches characters based on their
/// Unicode category.
/// </summary>
public sealed class UnicodeCategoryCharGroup
    : CharGroup
{
    private readonly UnicodeCategory[] categories;

    /// <summary>
    /// Gets the Unicode categories that this group matches.
    /// </summary>
    public string Selector {get;}

    /// <summary>
    /// Gets a value indicating whether this group is negated;
    /// that is, whether it matches characters that are not in the
    /// specified categories; or not.
    /// </summary>
    public bool Negated {get;}

    /// <summary>
    /// Initializes a new instance of the <see cref="UnicodeCategoryCharGroup"/> class.
    /// </summary>
    /// <param name="selector">
    /// The selector that specifies the Unicode categories that this group matches.
    /// </param>
    /// <param name="negated">
    /// <see langword="true"/> if this group is negated;
    /// otherwise, <see langword="false"/>.
    /// </param>
    public UnicodeCategoryCharGroup(string selector, bool negated = false)
    {
        Selector = selector;
        Negated = negated;

        this.categories = GetCategoriesFromSelector();
    }

    /// <inheritdoc/>
    public override bool TryMatch(int character)
    {
        var str = char.ConvertFromUtf32(character);
        var category = char.GetUnicodeCategory(str, 0);

        return categories.Contains(category) ^ Negated;
    }

    /// <inheritdoc/>
    public override string ToString()
        => Negated
            ? $"\\P{{{Selector}}}"
            : $"\\p{{{Selector}}}";

    private UnicodeCategory[] GetCategoriesFromSelector()
        => Selector.ParseAsUnicodeCategorySelector().ToArray();
}
