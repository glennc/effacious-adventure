using System;
using Xunit;
using JsonConfigParser;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace jsonConfigParserTests
{
    public class ParserTests
    {
        [Fact]
        public void CanParseSimpleJson()
        {
            var jsonString = "{ \"simpleKey\" : \"simpleValue\" }";
            using(var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(jsonString))))
            {
                var lexer = new JsonLexer(stream);
                var parser = new JsonParser(lexer);

                var parsedJson = parser.ParseObject();
                foreach(var item in parsedJson.Items)
                {
                    Console.WriteLine($"Key: {item.Key}, Value: {item.Value}");
                }
                
                Assert.True(parsedJson.Items.Keys.Count == 1);
                Assert.IsType<JsonString>(parsedJson.Items["simpleKey"]);
            }
        }

        [Theory]
        [InlineData("{ \"simpleKey\" : \"simpleValue\" }", typeof(JsonString))]
        [InlineData("{ \"simpleKey\" : false }", typeof(JsonLiteral))]
        [InlineData("{ \"simpleKey\" : \"\" }", typeof(JsonString))]
        [InlineData("{ \"simpleKey\" : [\"blah\", \"blah\"] }", typeof(JsonArray))]
        [InlineData("{ \"simpleKey\" : \"simpleValue\", \"simpleKey2\" : \"simpleValue2\" }", typeof(JsonString))]
        [InlineData("{ \"simpleKey\" : { \"simpleNestedKey\" : \"simpleNestedValue\" } }", typeof(JsonObject))]
        public void CanParseSimpleJson(string jsonString, Type jsonType)
        {
            using(var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(jsonString))))
            {
                var lexer = new JsonLexer(stream);
                var parser = new JsonParser(lexer);

                var parsedJson = parser.ParseObject();

                Assert.IsAssignableFrom<JsonType>(parsedJson.Items["simpleKey"]);
                Assert.True(parsedJson.Items["simpleKey"].GetType() == jsonType);
            }
        }

        [Fact]
        public void CanParseSimpleishProjectJson()
        {
            var json = @"{
    ""version"": ""1.0.0-*"",
    ""compilationOptions"": {
        ""emitEntryPoint"": false
    },
    ""dependencies"": {
        ""JsonConfigParser"" : """",
        ""Microsoft.NETCore.Runtime"": ""1.0.1-beta-*"",
        ""System.IO"": ""4.0.10-beta-*"",
        ""System.Console"": ""4.0.0-beta-*"",
        ""System.Runtime"": ""4.0.21-beta-*"",
        ""System.IO.FileSystem"": ""4.0.1-beta-*"",
        ""xunit"" : ""2.1.0"",
        ""xunit.runner.console"" : ""2.1.0""
    },
    ""frameworks"": {
        ""dnxcore50"": { }
    }
}";
            using(var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(json))))
            {
                var lexer = new JsonLexer(stream);
                var parser = new JsonParser(lexer);

                var parsedJson = parser.ParseObject();
                foreach(var item in parsedJson.Items)
                {
                    Console.WriteLine($"Key: {item.Key}, Value: {item.Value}");
                }
            }
        }
     }
}