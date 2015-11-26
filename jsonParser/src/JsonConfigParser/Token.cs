using System;

namespace JsonConfigParser
{
    public class Token
    {
        public int Type {get;set;}
        public char[] Value {get;set;}

        public Token(int type, params char[] chars)
        {
            Type = type;
            Value = chars;
        }

        public override string ToString()
        {
            return $"TOKEN<{Type}, {new string(Value)}>";
        }
    }
}