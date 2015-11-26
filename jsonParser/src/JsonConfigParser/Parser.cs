using System;
using System.Globalization;
using System.Text;

namespace JsonConfigParser
{
    public class JsonParser
    {
        private JsonLexer _lexer;
        private Token _currentToken;

        public JsonParser(JsonLexer lexer)
        {
            _lexer = lexer;
            _currentToken = lexer.GetNextToken();
        }

        private void Consume(int tokenType)
        {
            if(_currentToken.Type == tokenType)
            {
                _currentToken = _lexer.GetNextToken();
            }
            else
            {
                //Need to implement GetName method on token types and consider an enum
                throw new InvalidOperationException($"Unexpected token while parsing json expected: {tokenType}, Actual: {_currentToken.Type}");
            }
        }

        public JsonObject ParseObject()
        {
            Consume(TokenTypes.BEGIN_OBJECT);
            var jsonConfig = new JsonObject();
            while(_currentToken.Type != TokenTypes.END_OBJECT)
            {
                if(_currentToken.Type == TokenTypes.STRING)
                {
                    var key = ParseJsonString();
                    Consume(TokenTypes.NAME_SEPARATOR);
                    JsonType jsonValue = ParseJsonValue();
                    jsonConfig.AddJsonType(key, jsonValue);
                } 
                else if (_currentToken.Type == TokenTypes.VALUE_SEPARATOR)
                {
                    Consume(TokenTypes.VALUE_SEPARATOR);
                }
            }
            Consume(TokenTypes.END_OBJECT);
            return jsonConfig;
        }

        private JsonType ParseJsonValue()
        {
            if(_currentToken.Type == TokenTypes.STRING)
            {
                return new JsonString { Value = ParseJsonString() };
            }
            else if (_currentToken.Type == TokenTypes.BEGIN_OBJECT)
            {
                return ParseObject();
            }
            else if (_currentToken.Type == TokenTypes.BEGIN_ARRAY)
            {
                return ParseArray();
            }
            else if (_currentToken.Type == TokenTypes.LITERAL)
            {
                var literal = new JsonLiteral{Type = _currentToken.Type};
                Consume(TokenTypes.LITERAL);
                return literal;
            }
            throw new InvalidOperationException($"Invalid JSON Value tokens. Unable to parse {_currentToken}");
        }

        private JsonType ParseArray()
        {
            if(_currentToken.Type == TokenTypes.BEGIN_ARRAY)
            {
                var array = new JsonArray();
                Consume(TokenTypes.BEGIN_ARRAY);
                while(_currentToken.Type != TokenTypes.END_ARRAY)
                {
                    if(_currentToken.Type == TokenTypes.VALUE_SEPARATOR)
                    {
                        Consume(TokenTypes.VALUE_SEPARATOR);
                    }
                    array.Values.Add(ParseJsonValue());
                }
                Consume(TokenTypes.END_ARRAY);
                return array;
            }
            else
            {
                throw new InvalidOperationException($"Expected start of array char but got <{_currentToken.Type}>");
            }
        }

        private string ParseJsonString()
        {
            if(_currentToken.Type == TokenTypes.STRING)
            {
                var stringValue = new string(_currentToken.Value);
                Consume(TokenTypes.STRING);
                return stringValue;
            }
            else
            {
                throw new InvalidOperationException($"Expected string but instead got {_currentToken.Type}");
            }
        }
    }
}