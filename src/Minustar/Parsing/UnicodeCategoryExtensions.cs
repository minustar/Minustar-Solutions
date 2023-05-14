namespace Minustar.Parsing;

/// <summary>
/// Provides extension methods for the <see cref="UnicodeCategory"/> type.
/// </summary>
public static class UnicodeCategoryExtensions
{
    /// <summary>
    /// Returns the abbreviation of the specified <see cref="UnicodeCategory"/> value.
    /// </summary>
    /// <param name="value">
    /// The <see cref="UnicodeCategory"/> value to get the abbreviation of.
    /// </param>
    /// <returns>
    /// The abbreviation of the specified <see cref="UnicodeCategory"/> value.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="value"/> is not a valid <see cref="UnicodeCategory"/> value.
    /// </exception>
    public static string GetAbbreviation(this UnicodeCategory value)
        => value switch
        {
            Control                 => "Cc",
            Format                  => "Cf",
            OtherNotAssigned        => "Cn",
            PrivateUse              => "Co",
            Surrogate               => "Cs",
            LowercaseLetter         => "Ll",
            ModifierLetter          => "Lm",
            OtherLetter             => "Lo",
            TitlecaseLetter         => "Lt",
            UppercaseLetter         => "Lu",
            NonSpacingMark          => "Mn",
            SpacingCombiningMark    => "Mc",
            EnclosingMark           => "Me",
            DecimalDigitNumber      => "Nd",
            LetterNumber            => "Nl",
            OtherNumber             => "No",
            ConnectorPunctuation    => "Pc",
            DashPunctuation         => "Pd",
            ClosePunctuation        => "Pe",
            FinalQuotePunctuation   => "Pf",
            InitialQuotePunctuation => "Pi",
            OtherPunctuation        => "Po",
            OpenPunctuation         => "Ps",
            CurrencySymbol          => "Sc",
            ModifierSymbol          => "Sk",
            MathSymbol              => "Sm",
            OtherSymbol             => "So",
            LineSeparator           => "Zl",
            ParagraphSeparator      => "Zp",
            SpaceSeparator          => "Zs",

            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid UnicodeCategory value.")
        };
}
