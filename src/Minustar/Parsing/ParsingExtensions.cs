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
    /// <term>Esc.</term>
    /// <description>Character</description>
    /// </listheader>
    /// <item>
    /// <term><c>\0</c></term>
    /// <description>U+0000 NULL</description>
    /// </item>
    /// <item>
    /// <term><c>\b</c></term>
    /// <description>U+0008 BACKSPACE</description>
    /// </item>
    /// <item>
    /// <term><c>\t</c></term>
    /// <description>U+0009 HORIZONTAL TABULATION</description>
    /// </item>
    /// <item>
    /// <term><c>\n</c></term>
    /// <description>U+000A LINE FEED</description>
    /// </item>
    /// <item>
    /// <term><c>\v</c></term>
    /// <description>U+000B VERTICAL TABULATION</description>
    /// </item>
    /// <item>
    /// <term><c>\f</c></term>
    /// <description>U+000C FORM FEED</description>
    /// </item>
    /// <item>
    /// <term><c>\r</c></term>
    /// <description>U+000D CARRIAGE RETURN</description>
    /// </item>
    /// </list>
    /// <para>
    /// Some other characters are also escaped:
    /// </para>
    /// <list type="table">
    /// <listheader>
    /// <term>Esc.</term>
    /// <description>Character</description>
    /// </listheader>
    /// <item>
    /// <term><c>\\</c></term>
    /// <description>U+005C REVERSE SOLIDUS</description>
    /// </item>
    /// <item>
    /// <term><c>\"</c></term>
    /// <description>U+0022 QUOTATION MARK</description>
    /// </item>
    /// <item>
    /// <term><c>\'</c></term>
    /// <description>U+0027 APOSTROPHE</description>
    /// </item>
    /// </list>
    /// <para>
    /// Control characters, characters which code points are
    /// greater than U+FFFF are escaped with an hexadecimal escape
    /// sequence.
    /// </para>
    /// <list type="table">
    /// <listheader>
    /// <term>Esc.</term>
    /// <description>Character</description>
    /// </listheader>
    /// <item>
    /// <term><c>\x</c><em>XX</em></term>
    /// <description><em>XX</em> is two hexadecimal digits.</description>
    /// </item>
    /// <item>
    /// <term><c>\u</c><em>XXXX</em></term>
    /// <description><em>XXXX</em> is four hexadecimal digits.</description>
    /// </item>
    /// <item>
    /// <term><c>\u{</c><em>XX</em><c>}</c></term>
    /// <description><em>XX</em> is two to six hexadecimal digits.</description>
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
            index += length;
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
}
