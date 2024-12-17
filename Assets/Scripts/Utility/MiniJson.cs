using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
public static class MiniJson
{
    public static object Deserialize(string json)
    {
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonParser.Parse(json);
    }

    public static string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    private class JsonParser
    {
        private readonly string _json;
        private int _index;

        private JsonParser(string json)
        {
            _json = json;
        }

        public static object Parse(string json)
        {
            var parser = new JsonParser(json);
            return parser.ParseValue();
        }

        private object ParseValue()
        {
            SkipWhitespace();

            if (_index >= _json.Length)
                return null;

            char currentChar = _json[_index];
            switch (currentChar)
            {
                case '{':
                    return ParseObject();
                case '[':
                    return ParseArray();
                case '"':
                    return ParseString();
                default:
                    return ParseLiteral();
            }
        }

        private Dictionary<string, object> ParseObject()
        {
            var dict = new Dictionary<string, object>();
            ConsumeChar('{');
            SkipWhitespace();

            while (PeekChar() != '}')
            {
                string key = ParseString();
                SkipWhitespace();
                ConsumeChar(':');
                SkipWhitespace();
                object value = ParseValue();
                dict[key] = value;

                SkipWhitespace();
                if (PeekChar() == ',')
                    ConsumeChar(',');
                else
                    break;
            }

            ConsumeChar('}');
            return dict;
        }

        private List<object> ParseArray()
        {
            var list = new List<object>();
            ConsumeChar('[');
            SkipWhitespace();

            while (PeekChar() != ']')
            {
                object value = ParseValue();
                list.Add(value);

                SkipWhitespace();
                if (PeekChar() == ',')
                    ConsumeChar(',');
                else
                    break;
            }

            ConsumeChar(']');
            return list;
        }

        private string ParseString()
        {
            var sb = new StringBuilder();
            ConsumeChar('"');

            while (PeekChar() != '"')
            {
                char currentChar = ConsumeChar();
                if (currentChar == '\\')
                {
                    char escapedChar = ConsumeChar();
                    switch (escapedChar)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        default: throw new Exception($"Invalid escape character: {escapedChar}");
                    }
                }
                else
                {
                    sb.Append(currentChar);
                }
            }

            ConsumeChar('"');
            return sb.ToString();
        }

        private object ParseLiteral()
        {
            var literal = new StringBuilder();
            while (!IsAtEnd() && !char.IsWhiteSpace(PeekChar()) && PeekChar() != ',' && PeekChar() != '}' && PeekChar() != ']')
            {
                literal.Append(ConsumeChar());
            }

            string literalString = literal.ToString();
            if (literalString == "null") return null;
            if (literalString == "true") return true;
            if (literalString == "false") return false;

            if (double.TryParse(literalString, out var number))
                return number;

            throw new Exception($"Invalid literal: {literalString}");
        }

        private void SkipWhitespace()
        {
            while (!IsAtEnd() && char.IsWhiteSpace(PeekChar()))
                _index++;
        }

        private char PeekChar()
        {
            if (IsAtEnd())
                throw new Exception("Unexpected end of JSON.");
            return _json[_index];
        }

        private char ConsumeChar()
        {
            if (IsAtEnd())
                throw new Exception("Unexpected end of JSON.");
            return _json[_index++];
        }

        private void ConsumeChar(char expected)
        {
            SkipWhitespace();
            if (PeekChar() != expected)
                throw new Exception($"Expected '{expected}', but found '{PeekChar()}'");
            ConsumeChar();
        }

        private bool IsAtEnd()
        {
            return _index >= _json.Length;
        }
    }

    private class JsonSerializer
    {
        public static string Serialize(object obj)
        {
            if (obj == null)
                return "null";

            if (obj is string str)
                return $"\"{EscapeString(str)}\"";

            if (obj is bool boolean)
                return boolean ? "true" : "false";

            if (obj is IDictionary dictionary)
                return SerializeObject(dictionary);

            if (obj is IEnumerable enumerable)
                return SerializeArray(enumerable);

            return obj.ToString(); // Numbers
        }

        private static string SerializeObject(IDictionary dict)
        {
            var sb = new StringBuilder();
            sb.Append("{");

            bool first = true;
            foreach (var key in dict.Keys)
            {
                if (!first)
                    sb.Append(",");
                sb.Append($"\"{EscapeString(key.ToString())}\":{Serialize(dict[key])}");
                first = false;
            }

            sb.Append("}");
            return sb.ToString();
        }

        private static string SerializeArray(IEnumerable array)
        {
            var sb = new StringBuilder();
            sb.Append("[");

            bool first = true;
            foreach (var item in array)
            {
                if (!first)
                    sb.Append(",");
                sb.Append(Serialize(item));
                first = false;
            }

            sb.Append("]");
            return sb.ToString();
        }

        private static string EscapeString(string str)
        {
            return str.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
}