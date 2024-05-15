using System.Text;
using System.Text.RegularExpressions;

namespace aaspe_common.jsoncanonicalizer;

internal class JsonDecoder
{
    private const char LeftCurlyBracket = '{';
    private const char RightCurlyBracket = '}';
    private const char DoubleQuote = '"';
    private const char ColonCharacter = ':';
    private const char LeftBracket = '[';
    private const char RightBracket = ']';
    private const char CommaCharacter = ',';
    private const char BackSlash = '\\';

    private const string Sign = "-?";
    private const string IntegerPart = "[0-9]+";
    private const string DecimalPoint = @"\.";
    private const string DecimalPart = "[0-9]+";
    private const string ExponentPart = "([eE][-+]?[0-9]+)?";
    private const string NumberPatternString = $"^{Sign}{IntegerPart}({DecimalPoint}{DecimalPart})?{ExponentPart}?$";
    private static readonly Regex NumberPattern = new(NumberPatternString);

    private static readonly Regex BooleanPattern = new("^true|false$");

    private readonly string _jsonData;
    private JsonScanner _jsonScanner;

    internal readonly object? Root;

    public JsonDecoder(string jsonData)
    {
        _jsonData = jsonData;
        _jsonScanner = new JsonScanner(_jsonData);
        if (_jsonScanner.PeekNextNonWhiteSpaceCharacter() == LeftBracket)
        {
            _jsonScanner.Scan();
            Root = ParseArray();
        }
        else
        {
            _jsonScanner.ScanForNextCharacter(LeftCurlyBracket);
            Root = ParseObject();
        }

        while (_jsonScanner.IsIndexWithinJsonLength())
        {
            if (!_jsonScanner.IsNextCharacterWhiteSpace())
            {
                throw new IOException("Improperly terminated JSON object");
            }
        }
    }

    private object? ParseElement()
    {
        return _jsonScanner.Scan() switch
        {
            LeftCurlyBracket => ParseObject(),
            DoubleQuote => ParseQuotedString(),
            LeftBracket => ParseArray(),
            _ => ParseSimpleType()
        };
    }

    private object? ParseObject()
    {
        var dict =
            new SortedDictionary<string, object?>(StringComparer.Ordinal);
        var next = false;
        while (_jsonScanner.PeekNextNonWhiteSpaceCharacter() != RightCurlyBracket)
        {
            if (next)
            {
                _jsonScanner.ScanForNextCharacter(CommaCharacter);
            }

            next = true;
            _jsonScanner.ScanForNextCharacter(DoubleQuote);
            var name = ParseQuotedString();
            _jsonScanner.ScanForNextCharacter(ColonCharacter);
            dict.Add(name, ParseElement());
        }

        _jsonScanner.Scan();
        return dict;
    }

    private object? ParseArray()
    {
        var list = new List<object>();
        var next = false;
        while (_jsonScanner.PeekNextNonWhiteSpaceCharacter() != RightBracket)
        {
            if (next)
            {
                _jsonScanner.ScanForNextCharacter(CommaCharacter);
            }
            else
            {
                next = true;
            }

            list.Add(ParseElement());
        }

        _jsonScanner.Scan();
        return list;
    }

    private object? ParseSimpleType()
    {
        _jsonScanner.MoveBackToPreviousCharacter();
        var tempBuffer = new StringBuilder();
        char c;
        while ((c = _jsonScanner.PeekNextNonWhiteSpaceCharacter()) != CommaCharacter && c != RightBracket && c != RightCurlyBracket)
        {
            c = _jsonScanner.GetNextCharacter();
            if (char.IsWhiteSpace(c))
            {
                break;
            }

            tempBuffer.Append(c);
        }

        var token = tempBuffer.ToString();
        if (token.Length == 0)
        {
            throw new IOException("Missing argument");
        }

        if (NumberPattern.IsMatch(token))
        {
            return JsonConverter.ToDouble(token);
        }

        if (BooleanPattern.IsMatch(token))
        {
            return bool.Parse(token);
        }

        if (token.Equals("null"))
        {
            return null;
        }

        throw new IOException($"Unrecognized or malformed JSON token: {token}");
    }

    private string? ParseQuotedString()
    {
        var result = new StringBuilder();
        while (true)
        {
            var c = _jsonScanner.GetNextCharacter();
            if (c < ' ')
            {
                throw new IOException(c == '\n' ? "Unterminated string literal" : $"Unescaped control character: 0x{((int) c):x02}");
            }

            if (c == DoubleQuote)
            {
                break;
            }

            if (c == BackSlash)
            {
                c = HandleEscapeSequence();
            }

            result.Append(c);
        }

        return result.ToString();
    }

    private char HandleEscapeSequence()
    {
        var c = _jsonScanner.GetNextCharacter();
        switch (c)
        {
            case '"':
            case '\\':
            case '/':
                return c;

            case 'b':
                return '\b';

            case 'f':
                return '\f';

            case 'n':
                return '\n';

            case 'r':
                return '\r';

            case 't':
                return '\t';

            case 'u':
                var hexValue = 0;
                for (var i = 0; i < 4; i++)
                {
                    hexValue = (hexValue << 4) + GetHexChar();
                }

                return (char) hexValue;

            default:
                throw new IOException($"Unsupported escape: {c}");
        }
    }

    private char GetHexChar()
    {
        var c = _jsonScanner.GetNextCharacter();
        return c switch
        {
            '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9' => (char) (c - '0'),
            'a' or 'b' or 'c' or 'd' or 'e' or 'f' => (char) (c - 'a' + 10),
            'A' or 'B' or 'C' or 'D' or 'E' or 'F' => (char) (c - 'A' + 10),
            _ => throw new IOException($"Bad hex in \\u escape: {c}")
        };
    }
}