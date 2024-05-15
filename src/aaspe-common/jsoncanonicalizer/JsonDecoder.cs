﻿using System.Text;
using System.Text.RegularExpressions;
using Org.Webpki.JsonCanonicalizer;

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

    private static readonly Regex NumberPattern = new("^-?[0-9]+(\\.[0-9]+)?([eE][-+]?[0-9]+)?$");
    private static readonly Regex BooleanPattern = new("^true|false$");

    private int _index;
    private readonly string _jsonData;

    internal readonly object Root;

    public JsonDecoder(string jsonData)
    {
        _jsonData = jsonData;
        if (TestNextNonWhiteSpaceChar() == LeftBracket)
        {
            Scan();
            Root = ParseArray();
        }
        else
        {
            ScanFor(LeftCurlyBracket);
            Root = ParseObject();
        }

        while (_index < jsonData.Length)
        {
            if (!IsWhiteSpace(jsonData[_index++]))
            {
                throw new IOException("Improperly terminated JSON object");
            }
        }
    }

    private object ParseElement()
    {
        return Scan() switch
        {
            LeftCurlyBracket => ParseObject(),
            DoubleQuote => ParseQuotedString(),
            LeftBracket => ParseArray(),
            _ => ParseSimpleType()
        };
    }

    private object ParseObject()
    {
        var dict =
            new SortedDictionary<string, object>(StringComparer.Ordinal);
        var next = false;
        while (TestNextNonWhiteSpaceChar() != RightCurlyBracket)
        {
            if (next)
            {
                ScanFor(CommaCharacter);
            }

            next = true;
            ScanFor(DoubleQuote);
            var name = ParseQuotedString();
            ScanFor(ColonCharacter);
            dict.Add(name, ParseElement());
        }

        Scan();
        return dict;
    }

    private object ParseArray()
    {
        var list = new List<object>();
        var next = false;
        while (TestNextNonWhiteSpaceChar() != RightBracket)
        {
            if (next)
            {
                ScanFor(CommaCharacter);
            }
            else
            {
                next = true;
            }

            list.Add(ParseElement());
        }

        Scan();
        return list;
    }

    private object ParseSimpleType()
    {
        _index--;
        var tempBuffer = new StringBuilder();
        char c;
        while ((c = TestNextNonWhiteSpaceChar()) != CommaCharacter && c != RightBracket && c != RightCurlyBracket)
        {
            c = NextChar();
            if (IsWhiteSpace(c))
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

        throw new IOException("Unrecognized or malformed JSON token: " + token);
    }

    private string ParseQuotedString()
    {
        var result = new StringBuilder();
        while (true)
        {
            var c = NextChar();
            if (c < ' ')
            {
                throw new IOException(c == '\n' ? "Unterminated string literal" : "Unescaped control character: 0x" + ((int) c).ToString("x02"));
            }

            if (c == DoubleQuote)
            {
                break;
            }

            if (c == BackSlash)
            {
                switch (c = NextChar())
                {
                    case '"':
                    case '\\':
                    case '/':
                        break;

                    case 'b':
                        c = '\b';
                        break;

                    case 'f':
                        c = '\f';
                        break;

                    case 'n':
                        c = '\n';
                        break;

                    case 'r':
                        c = '\r';
                        break;

                    case 't':
                        c = '\t';
                        break;

                    case 'u':
                        c = (char) 0;
                        for (var i = 0; i < 4; i++)
                        {
                            c = (char) ((c << 4) + GetHexChar());
                        }

                        break;

                    default:
                        throw new IOException("Unsupported escape:" + c);
                }
            }

            result.Append(c);
        }

        return result.ToString();
    }

    private char GetHexChar()
    {
        var c = NextChar();
        return c switch
        {
            '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9' => (char) (c - '0'),
            'a' or 'b' or 'c' or 'd' or 'e' or 'f' => (char) (c - 'a' + 10),
            'A' or 'B' or 'C' or 'D' or 'E' or 'F' => (char) (c - 'A' + 10),
            _ => throw new IOException("Bad hex in \\u escape: " + c)
        };
    }

    private char TestNextNonWhiteSpaceChar()
    {
        var save = _index;
        var c = Scan();
        _index = save;
        return c;
    }

    private void ScanFor(char expected)
    {
        var c = Scan();
        if (c != expected)
        {
            throw new IOException("Expected '" + expected + "' but got '" + c + "'");
        }
    }

    private char NextChar()
    {
        if (_index < _jsonData.Length)
        {
            return _jsonData[_index++];
        }

        throw new IOException("Unexpected EOF reached");
    }

    private static bool IsWhiteSpace(char c)
    {
        return c == 0x20 || c == 0x0A || c == 0x0D || c == 0x09;
    }

    private char Scan()
    {
        while (true)
        {
            var c = NextChar();
            if (IsWhiteSpace(c))
            {
                continue;
            }

            return c;
        }
    }
}