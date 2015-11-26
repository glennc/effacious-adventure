using System;
using Xunit;
using JsonConfigParser;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace jsonConfigParserTests
{
    public class lexerTests
    {
        [Fact]
        public void EmptyStreamReturnsEOF()
        {
            using(var stream = new StreamReader(new MemoryStream()))
            {
                var lexerUnderTest = new JsonLexer(stream);
                var token = lexerUnderTest.GetNextToken();
                Assert.Equal(token.Type, TokenTypes.EOF);
            }
        }

        // [Fact]
        // public void CanIdentifyStrings()
        // {
        //     // var strings = new Dictionary<string,string>{
        //     //     {"\"This is a string\"", "\"This is a string\""},
        //     //     {"\"\\\\\"", "\"\\\""},
        //     //     {"\"\\u0041\"","\"\u0041\""},
        //     //     {"\"\\uD834\\uDD1E\"", "\"\U0001D11E\""}
        //     // };

        //     // foreach(var stringToTest in strings)
        //     // {
        //     //     Console.WriteLine($"Testing {stringToTest.Key}");
        //     //     using(var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringToTest.Key))))
        //     //     {
        //     //         var lexerUnderTest = new JsonLexer(stream);
        //     //         var token = lexerUnderTest.GetNextToken();
        //     //         Assert.Equal(token.Type, TokenTypes.STRING);
        //     //         Assert.Equal(new string(token.Value), stringToTest.Value);
        //     //         token = lexerUnderTest.GetNextToken();
        //     //         Assert.Equal(token.Type, TokenTypes.EOF);
        //     //     }
        //     // }
        // }

        [Fact]
        public void CanIdentifyStructuralChars()
        {
            var tokenString = "{}[]:,";
            var structuralTokenTypes = new int[] { TokenTypes.BEGIN_OBJECT, TokenTypes.END_OBJECT, TokenTypes.BEGIN_ARRAY, TokenTypes.END_ARRAY, TokenTypes.NAME_SEPARATOR, TokenTypes.VALUE_SEPARATOR };

            using(var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(tokenString))))
            {
                var lexerUnderTest = new JsonLexer(stream);

                for(int i = 0; i < structuralTokenTypes.Length; i++)
                {
                    var token = lexerUnderTest.GetNextToken();
                    Assert.Equal(token.Type, structuralTokenTypes[i]);
                }
            }
        }

        [Fact]
        public void CanIdentifyLiteralTokensAndIgnoresAllWhitespace()
        {
            var tokenString = "false true null";

            using(var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(tokenString))))
            {
                var lexerUnderTest = new JsonLexer(stream);
                var token = lexerUnderTest.GetNextToken();
                Assert.Equal(token.Type, TokenTypes.LITERAL);
                Assert.Equal(new string(token.Value), "false");
                token = lexerUnderTest.GetNextToken();
                Assert.Equal(token.Type, TokenTypes.LITERAL);
                Assert.Equal(new string(token.Value), "true");
                token = lexerUnderTest.GetNextToken();
                Assert.Equal(token.Type, TokenTypes.LITERAL);
                Assert.Equal(new string(token.Value), "null");
            }
        }
    }
}