using System;
using System.Text;

namespace ConsoleApplication
{
    public class Lexer
    {
        private string _source;
        private int _pos;
        private char? _currentChar;

        public Lexer(string input)
        {
            _source = input;
            _pos = -1;
            AdvancePos();
        }

        public void AdvancePos()
        {
            _pos++;
            if(_pos >= _source.Length)
            {
                _currentChar = null;
            }
            else
            {
                _currentChar = _source[_pos];
            }
        }

        public Token GetNextToken()
        {
            if(!_currentChar.HasValue)
            {
                return new Token(Token.EOI);
            }

            if(Char.IsWhiteSpace(_currentChar.Value))
            {
                AdvancePos();
                return GetNextToken();
            }

            if(char.IsDigit(_currentChar.Value))
            {
                StringBuilder buffer = new StringBuilder();
                while(_currentChar.HasValue && Char.IsDigit(_currentChar.Value))
                {
                    buffer.Append(_currentChar.Value);
                    AdvancePos();
                }
                return new Token(Token.INTEGER, buffer.ToString());
            }

            Token token = null;
            switch(_currentChar.Value)
            {
                case '+':
                    token = new Token(Token.ADD);
                    break;
                case '-':
                    token = new Token(Token.SUBTRACT);
                    break;
                case '/':
                    token = new Token(Token.DIVIDE);
                    break;
                case '*':
                    token = new Token(Token.MULTIPLY);
                    break;
                case '(':
                    token = new Token(Token.LPAREN);
                    break;
                case ')':
                    token = new Token(Token.RPAREN);
                    break;
            }

            if(token != null)
            {
                token.Source = _currentChar.Value.ToString();
                AdvancePos();
                return token;
            }
            else
            {
                throw new InvalidOperationException($"Unknown character {_currentChar.Value}");
            }
        }
    }
}