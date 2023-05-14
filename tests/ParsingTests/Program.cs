using Minustar.Parsing;

var hexDigitRule = Rule.Sequence(
    Rule.Optional(Rule.Literal("0x", true)),
    Rule.OneOrMany(
    Rule.Group(
        CharGroup.Range('0', '9'),
        CharGroup.Range('a', 'f'),
        CharGroup.Range('A', 'F')
    )));
Console.WriteLine(hexDigitRule);