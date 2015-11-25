using System;

namespace ConsoleApplication
{
    public class Parser
    {
        private Lexer _lexer;
        private Token _currentToken;

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currentToken = _lexer.GetNextToken();
        }

        public void Match(int tokenType)
        {
            if(_currentToken.Type == tokenType)
            {
                _currentToken = _lexer.GetNextToken();
            }
            else
            {
                throw new ArgumentException($"Invalid token type for this position of the equation. Expected {tokenType} but are parsing {_currentToken.Type}");
            }
        }

        public int Expr()
        {
            if(_currentToken.Type == Token.EOI)
            {
                return 0;
            }

            int result = Term();
            switch(_currentToken.Type)
            {
                case Token.ADD:
                    Match(Token.ADD);
                    result += Term();
                    break;
                case Token.SUBTRACT:
                    Match(Token.SUBTRACT);
                    result -= Term();
                    break;
            }
            return result;
        }

        private int Term()
        {
            int factor = Factor();

            switch(_currentToken.Type)
            {
                case Token.DIVIDE:
                    Match(Token.DIVIDE);
                    return factor /= Factor();
                case Token.MULTIPLY:
                    Match(Token.MULTIPLY);
                    return factor *= Factor();
            }

            return factor;
        }

        private int Factor()
        {
            if(_currentToken.Type == Token.INTEGER)
            {
                var integer = int.Parse(_currentToken.Source);
                Match(Token.INTEGER);
                return integer;
            }

            if(_currentToken.Type == Token.LPAREN)
            {
                var integer = 0;
                Match(Token.LPAREN);
                integer = Expr();
                Match(Token.RPAREN);
                return integer;
            }

            throw new ArgumentException("Invalid factor. Factors must either be an integer or an expression in parentheses.");
        }
    }
}