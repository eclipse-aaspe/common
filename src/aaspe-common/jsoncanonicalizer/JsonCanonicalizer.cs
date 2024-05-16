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
    StringBuilder buffer;

    public JsonCanonicalizer(string jsonData)
    {
        buffer = new StringBuilder();
        Serialize(new JsonDecoder().Decode(jsonData));
    }

    public JsonCanonicalizer(byte[] jsonData)
        : this(new UTF8Encoding(false, true).GetString(jsonData))
    {
    }

    private void SerializeString(string value)
    {
        var result = new StringBuilder();
        result.Append('"');

        foreach (var c in value)
        {
            EscapeCharacter(result, c);
        }

        result.Append('"');
        buffer.Append(result);
    }

    private void EscapeCharacter(StringBuilder result, char c)
    {
        var escapeSequences = new Dictionary<char, string>
        {
            { '\n', "\\n" },
            { '\b', "\\b" },
            { '\f', "\\f" },
            { '\r', "\\r" },
            { '\t', "\\t" },
            { '"', "\\\"" },
            { '\\', "\\\\" }
        };

        if (escapeSequences.TryGetValue(c, out var escapeSequence))
        {
            result.Append(escapeSequence);
        }
        else if (c < ' ')
        {
            result.Append("\\u").Append(((int)c).ToString("x04"));
        }
        else
        {
            result.Append(c);
        }
    }


    void Serialize(object? o)
    {
        if (o is SortedDictionary<string, object> objects)
        {
            buffer.Append('{');
            var next = false;
            foreach (var keyValuePair in objects)
            {
                if (next)
                {
                    buffer.Append(',');
                }

                next = true;
                SerializeString(keyValuePair.Key);
                buffer.Append(':');
                Serialize(keyValuePair.Value);
            }

            buffer.Append('}');
        }
        else if (o is List<object>)
        {
            buffer.Append('[');
            var next = false;
            foreach (var value in (List<object?>) o)
            {
                if (next)
                {
                    buffer.Append(',');
                }

                next = true;
                Serialize(value);
            }

            buffer.Append(']');
        }
        else if (o == null)
        {
            buffer.Append("null");
        }
        else if (o is string s)
        {
            SerializeString(s);
        }
        else if (o is bool)
        {
            buffer.Append(o.ToString()?.ToLowerInvariant());
        }
        else if (o is double)
        {
            buffer.Append(NumberToJson.SerializeNumber((double) o));
        }
        else
        {
            throw new InvalidOperationException($"Unknown object: {o}");
        }
    }

    public string GetEncodedString()
    {
        return buffer.ToString();
    }

    public IEnumerable<byte> GetEncodedUTF8()
    {
        return new UTF8Encoding(false, true).GetBytes(GetEncodedString());
    }
}