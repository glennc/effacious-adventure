using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;

namespace JsonConfigParser
{
    public class JsonLexer
    {
        private StreamReader _sourceStream;
        private int _lookaheadChar;
        //This is a buffer to re-use when reading in string values to return a string token.
        private List<char> _stringValueBuffer = new List<char>(20);
        //This buffer is for when we encounter unicode values such as \U0041. The JSON RFC specifies that
        //only 4 digit unicode values can be used so we can pre-allocate exactly enough space
        //to store the digits before we convert them to the correct value and add them to the 
        //string.
        private char[] _unicodeStringBuffer = new char[4];

        public JsonLexer(StreamReader source)
        {
            _sourceStream = source;
            AdvanceChar();
        }

        public void AdvanceChar()
        {
            //TODO: Once this is actually parsing full files it might be interesting to buffer line-by-line and
            //perhaps read in the next line asyncronously. Setup some timing tests to verify changes before...
            _lookaheadChar = _sourceStream.Read();
        }

        //Initially I had a structure like this, I tried a couple of different ones. Having this and looping through it
        //in GetNextToken consistently slowed down a token parse test by ~0.002 milliseconds.
        //I should look at it in a profiler at some point and see why that is the case. It is presumably something
        //that the compiler can do to optimize what is below vs the loop that the collection causes.
        // private static readonly Dictionary<char, int> _structuralTypeData = new Dictionary<char, int>{
        //     {'\x7B',TokenTypes.BEGIN_OBJECT}, // {
        //     {'\x7D',TokenTypes.END_OBJECT}, // }
        //     {'\x5B', TokenTypes.BEGIN_ARRAY}, //[
        //     {'\x5D', TokenTypes.END_ARRAY}, //]
        //     {'\x3A', TokenTypes.NAME_SEPARATOR}, //:
        //     {'\x2C', TokenTypes.VALUE_SEPARATOR} //,
        // };

        public Token GetNextToken()
        {
            if(_lookaheadChar == -1)
            {
                return new Token(TokenTypes.EOF);
            }

            //Skip whitespace chars.
            if(_lookaheadChar == '\x20' || //space
               _lookaheadChar == '\x09' || //Horizontal tab
               _lookaheadChar == '\x0A' || //Line feed or New line
               _lookaheadChar == '\x0D')   //Carriage return
            {
                AdvanceChar();
                return GetNextToken();
            }
            else if(_lookaheadChar == '\x7B') // LCBracket
            {
                AdvanceChar();
                return new Token(TokenTypes.BEGIN_OBJECT, '\x7B');
            }
            else if(_lookaheadChar == '\x7D') //RCBracket
            {
                AdvanceChar();
                return new Token(TokenTypes.END_OBJECT, '\x7D');
            }
            else if(_lookaheadChar == '\x5B') //LSBracket
            {
                AdvanceChar();
                return new Token(TokenTypes.BEGIN_ARRAY);
            }
            else if(_lookaheadChar == '\x5D') //RSBracket
            {
                AdvanceChar();
                return new Token(TokenTypes.END_ARRAY);
            }
            else if(_lookaheadChar == '\x3A') //:
            {
                AdvanceChar();
                return new Token(TokenTypes.NAME_SEPARATOR, '\x3A');
            }
            else if(_lookaheadChar == '\x2C') //,
            {
                AdvanceChar();
                return new Token(TokenTypes.VALUE_SEPARATOR, '\x2C');
            }
            //if the lookahead char is an f, n, or t then this is a literal, all other
            //types of value start with a different character.
            else if (_lookaheadChar == '\x66') //f, so this is false literal.
            {
                //TODO: This is obviously unsafe because I could write, fffff and it would still evaluate to false.
                //So I will come back and actually verify that each of these is the correct char.
                for(int i = 0; i <= 4; i++)
                {
                    AdvanceChar();
                }
                return new Token(TokenTypes.LITERAL, '\x66','\x61','\x6c','\x73','\x65');
            }
            else if (_lookaheadChar == '\x6e') //n, so this is null
            {
                for(int i = 0; i <= 3; i++)
                {
                    AdvanceChar();
                }
                return new Token(TokenTypes.LITERAL, '\x6e','\x75','\x6c','\x6c');
            }
            else if (_lookaheadChar == '\x74') //t, so true
            {
                for(int i = 0; i <= 3; i++)
                {
                    AdvanceChar();
                }
                return new Token(TokenTypes.LITERAL, '\x74', '\x72', '\x75', '\x65');
            }
            else if (_lookaheadChar == '\x22') //"
            {
                return new Token(TokenTypes.STRING, ReadString());
            }

            throw new InvalidOperationException($"Unable to tokenize <{(char)_lookaheadChar}>");
        }

        //I flipped back and forwards about putting this logic in the parser or the lexer.
        //In the end I put it here because of how specific it was to handline chars and doing escaping
        //It felt a little better for it to be here.
        private char[] ReadString()
        {
            //As per http://tools.ietf.org/html/rfc4627#section-2.5
            _stringValueBuffer.Clear();
            AdvanceChar();
            while(_lookaheadChar != '\x22')
            {
                // Escape chars with \
                if(_lookaheadChar == '\x5C')
                {
                    AdvanceChar();
                    // [", \, /] are all simple escape chars, just add them to the string.
                    if(_lookaheadChar == '\x22' ||
                       _lookaheadChar == '\x5C' ||
                       _lookaheadChar == '\x2F')
                    {
                        _stringValueBuffer.Add((char)_lookaheadChar);
                    }
                    // [b,f,n,r,t] are more special and require adding the appropriate char.
                    else if(_lookaheadChar == '\x62')
                    {
                        _stringValueBuffer.Add('\x08');
                    }
                    else if(_lookaheadChar == '\x66')
                    {
                        _stringValueBuffer.Add('\x0C');
                    }
                    else if(_lookaheadChar == '\x6E')
                    {
                        _stringValueBuffer.Add('\x0A');
                    }
                    else if(_lookaheadChar == '\x72')
                    {
                        _stringValueBuffer.Add('\x0D');
                    }
                    else if(_lookaheadChar == '\x74')
                    {
                        _stringValueBuffer.Add('\x09');
                    }
                    // [\u+] is a unicode sequence.
                    else if(_lookaheadChar == '\x75')
                    {
                        for(int i = 0; i <= 3; i++)
                        {
                            AdvanceChar();
                            _unicodeStringBuffer[i] = (char)_lookaheadChar;
                        }
                        //I don't like creating this string, maybe there is another way to do this...
                        var unicodeChar = (char)int.Parse(new string(_unicodeStringBuffer), NumberStyles.HexNumber);
                        _stringValueBuffer.Add(unicodeChar);
                    }
                }
                else
                {
                    _stringValueBuffer.Add((char)_lookaheadChar);
                }
                AdvanceChar();
            }
            AdvanceChar();
            return _stringValueBuffer.ToArray();
        }
    }
}