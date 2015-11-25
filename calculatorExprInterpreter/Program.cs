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
                    Console.Write("prompt>");
                    var input = Console.ReadLine();
                    Console.WriteLine("echo " + input);
                    Console.ReadLine();
                    
                    Console.WriteLine("calculator. Enter calculations:");
                    var text = Console.ReadLine();
                    Console.WriteLine($"Parsing {text}");
                    var lexer = new Lexer(text);
                    var interpreter = new Interpreter(lexer);
                    var result = interpreter.Expr();
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
        MULTIPLY = 16,
        LPARAM = 32,
        RPARAM = 64
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
    
    public class Lexer
    {
        public string SourceText {get;set;}
        private int _pos {get;set;}
        private char? _currentChar = null;
        
        public Lexer(string sourceText)
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
                string integerValue = string.Empty;
                while(_currentChar.HasValue && Char.IsNumber(_currentChar.Value))
                {
                    integerValue += _currentChar.ToString();
                    AdvanceChar();
                }
                return new Token(TokenType.INTEGER, integerValue);
            }

            Token knownToken = null;
            switch(_currentChar)
            {
                case '+':
                    knownToken = new Token(TokenType.PLUS, "+");
                    break;
                case '-':
                    knownToken = new Token(TokenType.MINUS, "-");
                    break;
                case '/':
                    knownToken = new Token(TokenType.DIVIDE, "/");
                    break;
                case '*':
                    knownToken = new Token(TokenType.MULTIPLY, "*");
                    break;
                case '(':
                    knownToken = new Token(TokenType.LPARAM, "(");
                    break;
                case ')':
                    knownToken = new Token(TokenType.RPARAM, ")");
                    break;
                default:
                    throw new ArgumentException($"Unable to parse {_currentChar.Value}");
            }

            AdvanceChar();
            return knownToken;
        }

    }

    public class Interpreter
    {
        public Token CurrentToken {get; set; }
        public Lexer Lexer {get; set;}

        public Interpreter(Lexer lexer)
        {
            Lexer = lexer;
            CurrentToken = Lexer.GetNextToken();
        }

        public bool Eat(TokenType tokenType)
        {
            if(tokenType.HasFlag(CurrentToken.TokenType))
            {
                this.CurrentToken = Lexer.GetNextToken();
                return true;
            }
            return false;
        }
        
        public int Factor()
        {
            var token = CurrentToken;
            if(token.TokenType == TokenType.LPARAM)
            {
                Eat(TokenType.LPARAM);
                var result = Expr();
                Eat(TokenType.RPARAM);
                return int.Parse(result);
            }
            Eat(TokenType.INTEGER);
            return int.Parse(token.TokenValue);
        }

        public int Term()
        {
            var termTokenTypes = TokenType.DIVIDE | TokenType.MULTIPLY;
            
            var result = Factor();
            while(termTokenTypes.HasFlag(CurrentToken.TokenType))
            {
                var token = CurrentToken;
                switch(token.TokenType)
                {
                    case TokenType.DIVIDE:
                        Eat(TokenType.DIVIDE);
                        result /= Factor();
                        break;
                    case TokenType.MULTIPLY:
                        Eat(TokenType.MULTIPLY);
                        result *= Factor();
                        break;
                }
            }
            return result;
        }

        public string Expr()
        {
            var opTokens = TokenType.PLUS | TokenType.MINUS;

            int result = Term();
            while(opTokens.HasFlag(CurrentToken.TokenType))
            {
                switch(CurrentToken.TokenType)
                {
                    case TokenType.PLUS:
                        Eat(TokenType.PLUS);
                        result += Term();
                        break;
                    case TokenType.MINUS:
                        Eat(TokenType.MINUS);
                        result -= Term();
                        break;
                    default:
                        throw new Exception($"Unknown operation: {CurrentToken.TokenType.ToString()}");
                }
            }

            return result.ToString();
        }
    }
}
