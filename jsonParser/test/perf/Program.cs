using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using JsonConfigParser;
using Microsoft.Extensions.JsonParser.Sources;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main2(string[] args)
        {
            for (int j = 0;  j < 1000; j++)
            {
                RunProjectParse();
            }
        }
        
        public static void Main(string[] args)
        {
            
            Console.WriteLine("TokenizerTest: START");
            Console.WriteLine("TokenizerTest: RawTokenTest: START");
            var testString = "\"This is a string\" null false true \"another string\" { } [ ] ,";
            var times = new List<double>();
            for(int i = 0; i < 5; i++)
            {
                times.Clear();
                for (int j = 0;  j < 1000; j++)
                {
                    using(var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(testString))))
                    {
                        var lexerUnderTest = new JsonLexer(stream);
                        var time = Time(() => {
                            Token token = null;
                            do
                            {
                                token = lexerUnderTest.GetNextToken();
                            } while(token.Type != TokenTypes.EOF);
                        });
                        times.Add(time.TotalMilliseconds);
                    }
                }
                Console.WriteLine($"TokenizerTest: {times.Average()}");
            }
            Console.WriteLine("TokenizerTest: RawTokenTest: STOP");
            Console.WriteLine("TokenizerTest: STOP");
            Console.WriteLine("ParserTest: START");
            Console.WriteLine("ParserTest: ProjectParse START");
            for(int i = 0; i < 5; i++)
            {
                times.Clear();
                for (int j = 0;  j < 1000; j++)
                {
                    var time = Time(RunProjectParse);
                    times.Add(time.TotalMilliseconds);
                }
                Console.WriteLine($"ParserTest: ProjectParse: {times.Average()}");
            }
            Console.WriteLine("ParserTest: ExistingParser START");
            for(int i = 0; i < 5; i++)
            {
                times.Clear();
                for (int j = 0;  j < 1000; j++)
                {
                    var time = Time(RunExistingProjectParse);
                    times.Add(time.TotalMilliseconds);
                }
                Console.WriteLine($"ParserTest: ProjectParse: {times.Average()}");
            }
            Console.WriteLine("ParseTest: ExistingParser STOP");
            Console.WriteLine("ParserTest: ProjectParse STOP");
        }

        public static void RunExistingProjectParse()
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
                var rawProject = JsonDeserializer.Deserialize(stream);
            }
        }

        public static void RunProjectParse()
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
            }
        }

        public static TimeSpan Time(Action action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
}
