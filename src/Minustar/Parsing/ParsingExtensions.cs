namespace Minustar.Parsing;

/// <summary>
/// Provides extension methods for the types in the
/// <see cref="Minustar.Parsing"/> namespace.
/// </summary>
public static class ParsingExtensions
{
    /// <summary>
    /// Escapes a string literal.
    /// </summary>
    /// <param name="str">
    /// The <see cref="string"/> to escape.
    /// </param>
    /// <returns>
    /// The escaped string literal.
    /// </returns>
    /// <remarks>
    /// <para>
    /// An escaped string literal is a string literal where
    /// control characters are escaped with backslash escape sequences.
    /// </para>
    /// <list type="table">
    /// <listheader>
    ///   <term>Esc.</term>
    ///   <description>Character</description>
    /// </listheader>
    /// <item>
    ///   <term><c>\0</c></term>
    ///   <description>U+0000 NULL</description>
    /// </item>
    /// <item>
    ///   <term><c>\b</c></term>
    ///   <description>U+0008 BACKSPACE</description>
    /// </item>
    /// <item>
    ///   <term><c>\t</c></term>
    ///   <description>U+0009 HORIZONTAL TABULATION</description>
    /// </item>
    /// <item>
    ///   <term><c>\n</c></term>
    ///   <description>U+000A LINE FEED</description>
    /// </item>
    /// <item>
    ///   <term><c>\v</c></term>
    ///   <description>U+000B VERTICAL TABULATION</description>
    /// </item>
    /// <item>
    ///   <term><c>\f</c></term>
    ///   <description>U+000C FORM FEED</description>
    /// </item>
    /// <item>
    ///   <term><c>\r</c></term>
    ///   <description>U+000D CARRIAGE RETURN</description>
    /// </item>
    /// </list>
    /// <para>
    /// Some other characters are also escaped:
    /// </para>
    /// <list type="table">
    /// <listheader>
    ///   <term>Esc.</term>
    ///   <description>Character</description>
    /// </listheader>
    /// <item>
    ///   <term><c>\\</c></term>
    ///   <description>U+005C REVERSE SOLIDUS</description>
    /// </item>
    /// <item>
    ///   <term><c>\"</c></term>
    ///   <description>U+0022 QUOTATION MARK</description>
    /// </item>
    /// <item>
    ///   <term><c>\'</c></term>
    ///   <description>U+0027 APOSTROPHE</description>
    /// </item>
    /// </list>
    /// <para>
    /// Control characters, characters which code points are
    /// greater than U+FFFF are escaped with an hexadecimal escape
    /// sequence.
    /// </para>
    /// <list type="table">
    /// <listheader>
    ///   <term>Esc.</term>
    ///   <description>Character</description>
    /// </listheader>
    /// <item>
    ///   <term><c>\x</c><em>XX</em></term>
    ///   <description><em>XX</em> is two hexadecimal digits.</description>
    /// </item>
    /// <item>
    ///   <term><c>\u</c><em>XXXX</em></term>
    ///   <description><em>XXXX</em> is four hexadecimal digits.</description>
    /// </item>
    /// <item>
    ///   <term><c>\u{</c><em>XX</em><c>}</c></term>
    ///   <description><em>XX</em> is two to six hexadecimal digits.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public static string EscapeStringLiteral(this string str)
    {
        if (str is not { Length: > 0 })
            return str;

        var builder = new StringBuilder();
        int length = 0;
        for (int index = 0; index < str.Length; index += length)
        {
            var codePoint = str.GetCodePoint(index, out length);
            builder.Append(codePoint switch
            {
                // Common control characters.
                '\0' => "\\0",
                '\b' => "\\b",
                '\t' => "\\t",
                '\n' => "\\n",
                '\v' => "\\v",
                '\f' => "\\f",
                '\r' => "\\r",

                // Escaped  characters.
                '\\' => "\\\\",
                '\"' => "\\\"",
                '\'' => "\\\'",

                // Characters that are control characters
                _ when char.IsControl(str, index) => $"\\u{codePoint:X4}",

                // Characters that are greater than U+FFFF.
                _ when codePoint > 0xFFFF => $"\\u{{{codePoint:X2}}}",

                // All other characters.
                _ => char.ConvertFromUtf32(codePoint)
            });
        }
        return builder.ToString();
    }

    /// <summary>
    /// Un-escapes a string literal.
    /// </summary>
    /// <param name="str">
    /// The <see cref="string"/> to un-escape.
    /// </param>
    /// <returns>
    /// The un-escaped string literal.
    /// </returns>
    /// <seealso cref="EscapeStringLiteral(string)"/>
    public static string UnescapeStringLiteral(this string str)
    {
        if (str is not { Length: > 0 })
            return str;

        var builder = new StringBuilder();
        int lengtH = 0;
        for (int index = 0; index < str.Length; index += lengtH)
        {
            var codePoint = str.GetCodePoint(index, out lengtH);
            if (codePoint != '\\')
            {
                builder.Append(char.ConvertFromUtf32(codePoint));
                continue;
            }

            // Let's get the next character.
            index += lengtH;
            codePoint = str.GetCodePoint(index, out lengtH);

            // If it's a common escape character, we can just append it.
            if (TryConvertCommonEscapeCharacter(codePoint, out var unescaped))
            {
                builder.Append(unescaped);
                continue;
            }

            // It's it's an 'x' or 'X'.
            if (codePoint == 'x' || codePoint == 'X')
            {
                // If the next two characters are hex digits,
                // we convert that into a character.
                if (str.IsHexDigit(index + 1) && str.IsHexDigit(index + 2))
                {
                    var hex = str.Substring(index + 1, 2);
                    builder.Append(char.ConvertFromUtf32(int.Parse(hex, HexNumber)));
                    index += 2;
                    continue;
                }
            }

            // If it's an 'u' or 'U', we first check if the next character is a '{'.
            // If not, we try to read four hex digits.
            // Otherwise, we try to read two to six hex digits, until '}'.
            // Once the hex digit is read, we convert it into a character.
            if (codePoint == 'u' || codePoint == 'U')
            {
                if (str.GetCodePoint(index + 1, out lengtH) == '{')
                {
                    index += lengtH;
                    var hex = new StringBuilder();
                    while (str.GetCodePoint(index, out lengtH) is var c && c != '}')
                    {
                        if (!str.IsHexDigit(index))
                            throw new FormatException($"Invalid hex digit at index {index}.");
                        hex.Append(char.ConvertFromUtf32(c));
                        index += lengtH;
                    }
                    if (hex.Length is < 2 or > 6)
                        throw new FormatException($"Invalid hex digit at index {index}.");
                    builder.Append(char.ConvertFromUtf32(int.Parse(hex.ToString(), HexNumber)));
                    continue;
                }
                else if (str.Check(IsHexDigit, index + 1, 4))
                {
                    var hex = str.Substring(index + 1, 4);
                    builder.Append(char.ConvertFromUtf32(int.Parse(hex, HexNumber)));
                    index += 4;
                    continue;
                }
            }
        }
        return builder.ToString();
    }

    private static bool IsHexDigit(char c)
        => (c >= '0' && c <= '9')
        || (c >= 'a' && c <= 'f')
        || (c >= 'A' && c <= 'F');

    private static bool TryConvertCommonEscapeCharacter(int codePoint, [NotNullWhen(true)] out string? unescaped)
    {
        unescaped = codePoint switch
        {
            '0' => "\0",
            'b' => "\b",
            't' => "\t",
            'n' => "\n",
            'v' => "\v",
            'f' => "\f",
            'r' => "\r",
            '\\' => "\\",
            '\"' => "\"",
            '\'' => "\'",
            _ => null
        };
        return unescaped is not null;
    }

    /// <summary>
    /// Parses a <see cref="string"/> as a selector for a set of
    /// <see cref="UnicodeCategory"/> values.
    /// </summary>
    /// <param name="selector">
    /// The <see cref="string"/> to parse.
    /// </param>
    /// <returns>
    /// The <see cref="UnicodeCategory"/> values that the selector
    /// represents.
    /// </returns>
    /// <remarks>
    /// <para>
    /// A selector is a <see cref="string"/> that represents a set of
    /// <see cref="UnicodeCategory"/> values using their abbreviations.
    /// </para>
    /// <para>
    /// A selector can either either one character long, or two
    /// characters long. The second character can be a wildcard
    /// character, e.g. <c>C*</c> is equivalent to <c>C</c> and
    /// will match every category which abbreviation starts with
    /// <c>C</c>.
    /// </para>
    /// <para>
    /// The following abbreviations are supported:
    /// </para>
    /// <list type="table">
    /// <listheader>
    ///   <term>Abbreviation</term>
    ///   <description>Unicode category</description>
    /// </listheader>
    /// <item>
    ///   <term><c>C</c> or <c>C*</c></term>
    ///   <description>Matches <c>Cc</c>, <c>Cf</c>, <c>Cn</c>, and <c>Co</c>.</description>
    /// </item>
    /// <item>
    ///   <term><c>Cc</c></term>
    ///   <description>Matches <em>Control</em> <c>(Cc)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Cf</c></term>
    ///   <description>Matches <em>Format</em> <c>(Cf)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Cn</c></term>
    ///   <description>Matches <em>Unassigned</em> <c>(Cn)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Co</c></term>
    ///   <description>Matches <em>Private Use</em> <c>(Co)</c> and <em>Surrogate</em> <c>(Co)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>L</c> or <c>L*</c></term>
    ///   <description>Matches <c>Ll</c>, <c>Lm</c>, <c>Lo</c>, <c>Lt</c>, and <c>Lu</c>.</description>
    /// </item>
    /// <item>
    ///   <term><c>Ll</c></term>
    ///   <description>Matches <em>Lowercase Letter</em> <c>(Ll)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Lm</c></term>
    ///   <description>Matches <em>Modifier Letter</em> <c>(Lm)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Lo</c></term>
    ///   <description>Matches <em>Other Letter</em> <c>(Lo)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Lt</c></term>
    ///   <description>Matches <em>Titlecase Letter</em> <c>(Lt)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Lu</c></term>
    ///   <description>Matches <em>Uppercase Letter</em> <c>(Lu)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>M</c> or <c>M*</c></term>
    ///   <description>Matches <c>Mc</c>, <c>Me</c>, and <c>Mn</c>.</description>
    /// </item>
    /// <item>
    ///   <term><c>Mc</c></term>
    ///   <description>Matches <em>Spacing Combining Mark</em> <c>(Mc)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Me</c></term>
    ///   <description>Matches <em>Enclosing Mark</em> <c>(Me)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Mn</c></term>
    ///   <description>Matches <em>Non-Spacing Mark</em> <c>(Mn)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>N</c> or <c>N*</c></term>
    ///   <description>Matches <c>Nd</c>, <c>Nl</c>, and <c>No</c>.</description>
    /// </item>
    /// <item>
    ///   <term><c>Nd</c></term>
    ///   <description>Matches <em>Decimal Digit Number</em> <c>(Nd)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Nl</c></term>
    ///   <description>Matches <em>Letter Number</em> <c>(Nl)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>No</c></term>
    ///   <description>Matches <em>Other Number</em> <c>(No)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>P</c> or <c>P*</c></term>
    ///   <description>Matches <c>Pc</c>, <c>Pd</c>, <c>Pe</c>, <c>Pf</c>, <c>Pi</c>, <c>Po</c>, and <c>Ps</c>.</description>
    /// </item>
    /// <item>
    ///   <term><c>Pc</c></term>
    ///   <description>Matches <em>Connector Punctuation</em> <c>(Pc)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Pd</c></term>
    ///   <description>Matches <em>Dash Punctuation</em> <c>(Pd)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Pe</c></term>
    ///   <description>Matches <em>Close Punctuation</em> <c>(Pe)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Pf</c></term>
    ///   <description>Matches <em>Final Punctuation</em> <c>(Pf)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Pi</c></term>
    ///   <description>Matches <em>Initial Punctuation</em> <c>(Pi)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Po</c></term>
    ///   <description>Matches <em>Other Punctuation</em> <c>(Po)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Ps</c></term>
    ///   <description>Matches <em>Open Punctuation</em> <c>(Ps)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>S</c> or <c>S*</c></term>
    ///   <description>Matches <c>Sc</c>, <c>Sk</c>, <c>Sm</c>, <c>So</c>.</description>
    /// </item>
    /// <item>
    ///   <term><c>Sc</c></term>
    ///   <description>Matches <em>Currency Symbol</em> <c>(Sc)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Sk</c></term>
    ///   <description>Matches <em>Modifier Symbol</em> <c>(Sk)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Sm</c></term>
    ///   <description>Matches <em>Math Symbol</em> <c>(Sm)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>So</c></term>
    ///   <description>Matches <em>Other Symbol</em> <c>(So)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Z</c> or <c>Z*</c></term>
    ///   <description>Matches <c>Zl</c>, <c>Zp</c>, and <c>Zs</c>.</description>
    /// </item>
    /// <item>
    ///   <term><c>Zl</c></term>
    ///   <description>Matches <em>Line Separator</em> <c>(Zl)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Zp</c></term>
    ///   <description>Matches <em>Paragraph Separator</em> <c>(Zp)</c> characters.</description>
    /// </item>
    /// <item>
    ///   <term><c>Zs</c></term>
    ///   <description>Matches <em>Space Separator</em> <c>(Zs)</c> characters.</description>
    /// </item>
    /// </list>
    /// <para>
    /// Note that the matching is done while ignoring case.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Throw when <paramref name="selector"/> is not one or two characters long.
    /// </exception>
    public static IEnumerable<UnicodeCategory> ParseAsUnicodeCategorySelector(this string selector)
    {
        if (selector is not { Length: 1 or 2 })
            throw new InvalidOperationException();

        foreach (var category in Enum.GetValues<UnicodeCategory>())
            if (category.MatchesSelector(selector))
                yield return category;
    }

    /// <summary>
    /// Checks whether a <see cref="UnicodeCategory"/> value matches
    /// a selector.
    /// </summary>
    /// <param name="value">
    /// The <see cref="UnicodeCategory"/> value to check.
    /// </param>
    /// <param name="selector">
    /// The selector to check.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> matches
    /// the <paramref name="selector"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool MatchesSelector(this UnicodeCategory value, string selector)
    {
        var abbreviation = value.GetAbbreviation().ToLowerInvariant();
        selector = selector.ToLowerInvariant();

        if (abbreviation is not string { Length: 2 })
            throw new InvalidOperationException();

        if (selector is not string { Length: 1 or 2 })
            throw new InvalidOperationException();

        bool matchMajor = selector[0] == abbreviation[0];
        bool matchMinor = selector.Length == 1
            || selector[1] == '*'
            || selector[1] == abbreviation[1];

        return matchMajor && matchMinor;
    }
}
