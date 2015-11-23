using System;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            while(true)
            {
                try
                {
                    Console.WriteLine("calculator. Enter calculations:");
                    var text = Console.ReadLine();
                    Console.WriteLine($"Parsing {text}");
                    var interpreter = new Interpreter(text);
                    var result = interpreter.Execute();
                    Console.WriteLine($"Result: {result.ToString()}");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }

    [Flags]
    public enum TokenType
    {
        INTEGER = 0,
        PLUS = 1,
        MINUS = 2,
        EOF = 4,
        DIVIDE = 8,
        MULTIPLY = 16
    }

    public class Token
    {
        public TokenType TokenType {get;set;}
        public string TokenValue {get;set;}
        
        public Token(TokenType tokenType) :
            this(tokenType, null)
        {
        }
        
        public Token(TokenType tokenType, string tokenValue)
        {
            this.TokenType = tokenType;
            this.TokenValue = tokenValue;
        }
        
        public override string ToString()
        {
            return $"Token({TokenType}, {TokenValue})";
        }
    }
    
    public class Interpreter
    {
        public string SourceText {get;set;}
        private int _pos {get;set;}
        private char? _currentChar = null;
        public Token CurrentToken {get; set; }
        
        
        public Interpreter(string sourceText)
        {
            this.SourceText = sourceText;
            _pos = -1;
            AdvanceChar();
        }
        
        public void AdvanceChar()
        {
            _pos++;
            if(_pos >= this.SourceText.Length)
            {
                _currentChar = null;
            }
            else
            {
                _currentChar = SourceText[_pos];
            }
            
        }
        
        public Token GetNextToken()
        {
            if(!_currentChar.HasValue)
            {
                return new Token(TokenType.EOF);
            }

            if(Char.IsWhiteSpace(_currentChar.Value))
            {
                AdvanceChar();
                return GetNextToken();
            }

            if(Char.IsDigit(_currentChar.Value))
            {
                //Some refactoring for recursion here is probably better...
                string integerValue = string.Empty;
                while(_currentChar.HasValue && Char.IsNumber(_currentChar.Value))
                {
                    integerValue += _currentChar.ToString();
                    AdvanceChar();
                }
                return new Token(TokenType.INTEGER, integerValue);
            }
            
            TokenType? opTokenType = null;
            switch(_currentChar)
            {
                case '+':
                    opTokenType = TokenType.PLUS;
                    break;
                case '-':
                    opTokenType = TokenType.MINUS;
                    break;
                case '/':
                    opTokenType = TokenType.DIVIDE;
                    break;
                case '*':
                    opTokenType = TokenType.MULTIPLY;
                    break;
            }

            if(opTokenType.HasValue)
            {
                //TODO: I dislike having to do this, once the code has diverged from the Python
                //source enough to make similarity a non-goal it should be changed.
                var tokenChar = _currentChar;
                AdvanceChar();
                return new Token(opTokenType.Value, tokenChar.ToString());
            }
            throw new ArgumentException($"Unable to parse {_currentChar.Value}");
        }

        public bool Eat(TokenType tokenType)
        {
            if(tokenType.HasFlag(CurrentToken.TokenType))
            {
                this.CurrentToken = this.GetNextToken();
                return true;
            }
            return false;
        }
        
        public int UnwrapTerm()
        {
            var token = CurrentToken;
            Eat(TokenType.INTEGER);
            return int.Parse(token.TokenValue);
        }

        public string Execute()
        {
            var opTokens = TokenType.PLUS | TokenType.DIVIDE | TokenType.MINUS | TokenType.MULTIPLY;
            this.CurrentToken = this.GetNextToken();
            int result = UnwrapTerm();
            while(opTokens.HasFlag(CurrentToken.TokenType))
            {
                 switch(CurrentToken.TokenType)
                {
                    case TokenType.PLUS:
                        Eat(TokenType.PLUS);
                        result += UnwrapTerm();
                        break;
                    case TokenType.MINUS:
                        Eat(TokenType.MINUS);
                        result -= UnwrapTerm();
                        break;
                    case TokenType.DIVIDE:
                        Eat(TokenType.DIVIDE);
                        result /= UnwrapTerm();
                        break;
                    case TokenType.MULTIPLY:
                        Eat(TokenType.MULTIPLY);
                        result *= UnwrapTerm();
                        break;
                    default:
                        throw new Exception($"Unknown operation: {CurrentToken.TokenType.ToString()}");
                }
            }

            return result.ToString();
        }
    }
}
