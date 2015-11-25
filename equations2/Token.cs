using System;
using System.Collections.Generic;

namespace ConsoleApplication
{
    public class Token
    {
        public int Type {get;set;}
        public string Source {get;set;}

        public const int EOI = 0;
        public const int INTEGER = 1;
        public const int ADD = 2;
        public const int SUBTRACT = 3;
        public const int MULTIPLY = 4;
        public const int DIVIDE = 5;
        public const int LPAREN = 6;
        public const int RPAREN = 7;

        public static readonly string[] TokenTypes = {
                "EOI",
                "INTEGER",
                "ADD",
                "SUBTRACT",
                "MULTIPLY",
                "DIVIDE",
                "LPAREN",
                "RPAREN"
        };

        public Token(int type) : this(type, null)
        {
        }

        public Token(int type, string source)
        {
            Type = type;
            Source = source;
        }

        public override string ToString()
        {
            return $"TOKEN <Type: {GetTokenName(Type)}, Source: {Source}>";
        }

        public string GetTokenName(int tokenType)
        {
            return TokenTypes[tokenType];
        }

    }
}