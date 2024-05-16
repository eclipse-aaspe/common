/*
 *  Copyright 2006-2019 WebPKI.org (http://webpki.org).
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      https://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */

using System.Text;
using aaspe_common.jsoncanonicalizer;
using Org.Webpki.Es6NumberSerialization;

namespace Org.Webpki.JsonCanonicalizer;

public class JsonCanonicalizer
{
    private const string NullValue = "null";
    private const char ObjectStart = '{';
    private const char ObjectEnd = '}';
    private const char ArrayStart = '[';
    private const char ArrayEnd = ']';
    private const char Comma = ',';
    private const char Colon = ':';
    private const char Quote = '"';
    private const string UnicodeFormat = "x04";
    private const string UnicodePrefix = "\\u";

    private readonly StringBuilder _buffer;
    private Dictionary<Type, Action<object>> _serializationActions;

    private static readonly Dictionary<char, string> EscapeSequences = new()
    {
        { '\n', "\\n" },
        { '\b', "\\b" },
        { '\f', "\\f" },
        { '\r', "\\r" },
        { '\t', "\\t" },
        { '"', "\\\"" },
        { '\\', "\\\\" }
    };

    public JsonCanonicalizer(string jsonData)
    {
        _buffer = new StringBuilder();
        InitializeSerializationActions();
        Serialize(new JsonDecoder().Decode(jsonData));
    }

    public JsonCanonicalizer(byte[] jsonData)
        : this(new UTF8Encoding(false, true).GetString(jsonData))
    {
    }

    public string GetEncodedString()
    {
        return _buffer.ToString();
    }

    public IEnumerable<byte> GetEncodedUTF8()
    {
        return new UTF8Encoding(false, true).GetBytes(GetEncodedString());
    }

    private void InitializeSerializationActions()
    {
        _serializationActions = new Dictionary<Type, Action<object>>
        {
            { typeof(SortedDictionary<string, object>), o => SerializeObject((SortedDictionary<string, object>)o) },
            { typeof(List<object>), o => SerializeArray((List<object?>)o) },
            { typeof(string), o => SerializeString((string)o) },
            { typeof(bool), o => _buffer.Append(o.ToString()?.ToLowerInvariant()) },
            { typeof(double), o => _buffer.Append(NumberToJson.SerializeNumber((double)o)) }
        };
    }

    private void Serialize(object? o)
    {
        if (o == null)
        {
            _buffer.Append(NullValue);
            return;
        }

        var objectType = o.GetType();
        if (_serializationActions.ContainsKey(objectType))
        {
            _serializationActions[objectType](o);
        }
        else
        {
            throw new InvalidOperationException($"Unknown object type: {objectType}");
        }
    }

    private void SerializeObject(SortedDictionary<string, object> objects)
    {
        _buffer.Append(ObjectStart);
        var next = false;
        foreach (var keyValuePair in objects)
        {
            if (next) _buffer.Append(Comma);
            next = true;
            SerializeString(keyValuePair.Key);
            _buffer.Append(Colon);
            Serialize(keyValuePair.Value);
        }
        _buffer.Append(ObjectEnd);
    }

    private void SerializeArray(List<object?> array)
    {
        _buffer.Append(ArrayStart);
        var next = false;
        foreach (var item in array)
        {
            if (next) _buffer.Append(Comma);
            next = true;
            Serialize(item);
        }
        _buffer.Append(ArrayEnd);
    }

    private void SerializeString(string value)
    {
        var result = new StringBuilder();
        result.Append(Quote);

        foreach (var c in value)
        {
            EscapeCharacter(result, c);
        }

        result.Append(Quote);
        _buffer.Append(result);
    }

    private void EscapeCharacter(StringBuilder result, char c)
    {
        if (EscapeSequences.TryGetValue(c, out var escapeSequence))
        {
            result.Append(escapeSequence);
        }
        else if (c < ' ')
        {
            result.Append(UnicodePrefix).Append(((int)c).ToString(UnicodeFormat));
        }
        else
        {
            result.Append(c);
        }
    }
}
