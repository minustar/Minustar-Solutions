namespace Minustar.Parsing;

/// <summary>
/// Specifies a rule that can be matched against a <see cref="string"/>.
/// </summary>
public interface IRule
{
    /// <summary>
    /// Tries to match the rule against the given <see cref="string"/>
    /// at the given index.
    /// </summary>
    /// <param name="str">
    /// The <see cref="string"/> to match against.
    /// </param>
    /// <param name="index">
    /// The index to start matching at.
    /// </param>
    /// <param name="length">
    /// The length of the match, if successful.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the rule matched;
    /// otherwise <see langword="false"/>.
    /// </returns>
    bool TryMatch(string str, int index, out int length);
}
