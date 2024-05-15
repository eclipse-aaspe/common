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
        Serialize(new JsonDecoder(jsonData).Root);
    }

    public JsonCanonicalizer(byte[] jsonData)
        : this(new UTF8Encoding(false, true).GetString(jsonData))
    {

    }

    private void Escape(char c)
    {
        buffer.Append('\\').Append(c);
    }

    private void SerializeString(string value)
    {
        buffer.Append('"');
        foreach (char c in value)
        {
            switch (c)
            {
                case '\n':
                    Escape('n');
                    break;

                case '\b':
                    Escape('b');
                    break;

                case '\f':
                    Escape('f');
                    break;

                case '\r':
                    Escape('r');
                    break;

                case '\t':
                    Escape('t');
                    break;

                case '"':
                case '\\':
                    Escape(c);
                    break;

                default:
                    if (c < ' ')
                    {
                        buffer.Append("\\u").Append(((int)c).ToString("x04"));
                    }
                    else
                    {
                        buffer.Append(c);
                    }
                    break;
            }
        }
        buffer.Append('"');
    }

    void Serialize(object o)
    {
        if (o is SortedDictionary<string, object>)
        {
            buffer.Append('{');
            bool next = false;
            foreach (var keyValuePair in (SortedDictionary<string, object>)o)
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
            bool next = false;
            foreach (object value in (List<object>)o)
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
        else if (o is String)
        {
            SerializeString((string)o);
        }
        else if (o is Boolean)
        {
            buffer.Append(o.ToString().ToLowerInvariant());
        }
        else if (o is Double)
        {
            buffer.Append(NumberToJson.SerializeNumber((Double)o));
        }
        else
        {
            throw new InvalidOperationException("Unknown object: " + o);
        }
    }

    public string GetEncodedString()
    {
        return buffer.ToString();
    }

    public byte[] GetEncodedUTF8()
    {
        return new UTF8Encoding(false, true).GetBytes(GetEncodedString());
    }
}